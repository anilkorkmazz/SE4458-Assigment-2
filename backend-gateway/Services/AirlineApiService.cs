using backend_gateway.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace backend_gateway.Services
{
    public class AirlineApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        private string _cachedToken;
        private DateTime _tokenExpiry;

        public AirlineApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private async Task<string> GetJwtTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
                return _cachedToken;

            var loginPayload = new LoginRequestDto
            {
                Username = "admin",
                Password = "123"
            };

            var loginUrl = $"{_config["Gateway:BaseUrl"]}/api/login";  

            var response = await _httpClient.PostAsJsonAsync(loginUrl, loginPayload);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Login failed: " + content);

            var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            _cachedToken = parsed["token"];
            _tokenExpiry = DateTime.UtcNow.AddMinutes(10);
            return _cachedToken;
        }

        public async Task<HttpResponseMessage> CallAirlineApiAsync(AiResponseDto dto)
        {
            var token = await GetJwtTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var baseUrl = _config["Gateway:BaseUrl"]; 

            switch (dto.Intent)
            {
                case "QueryFlight":
                    return await _httpClient.PostAsJsonAsync($"{baseUrl}/api/flight/query", new
                    {
                        dto.DateFrom,
                        dto.DateTo,
                        dto.AirportFrom,
                        dto.AirportTo,
                        dto.NumberOfPeople,
                        dto.IsRoundTrip,
                        dto.PageNumber
                    });

                case "BuyTicket":
                    return await _httpClient.PostAsJsonAsync($"{baseUrl}/api/ticket/buy", new
                    {
                        dto.FlightNumber,
                        dto.Date,
                        dto.PassengerNames
                    });

                case "CheckIn":
                    return await _httpClient.PostAsJsonAsync($"{baseUrl}/api/ticket/checkin", new
                    {
                        dto.FlightNumber,
                        dto.Date,
                        dto.PassengerName
                    });

                case "QueryPassengerList":
                    string url = $"{baseUrl}/api/ticket/passenger-list/{dto.FlightNumber}/{dto.Date}/{dto.PageNumber}";
                    return await _httpClient.GetAsync(url);

                default:
                    throw new Exception("Unknown intent: " + dto.Intent);
            }
        }
    }
}
