using backend_gateway.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace backend_gateway.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public OpenAiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            _config = config;
        }

        public async Task<AiResponseDto> GetIntentAsync(string message)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = @"
Extract the user's intent and parameters from the natural language input.

You can only use the following 4 intents:
- QueryFlight
- BuyTicket
- CheckIn
- QueryPassengerList

Return only the matching JSON structure for each intent:

1. QueryFlight:
{
  ""intent"": ""QueryFlight"",
  ""dateFrom"": ""YYYY-MM-DDTHH:MM:SS"",
  ""dateTo"": ""YYYY-MM-DDTHH:MM:SS"",
  ""airportFrom"": ""IATA_CODE"",
  ""airportTo"": ""IATA_CODE"",
  ""numberOfPeople"": NUMBER,
  ""isRoundTrip"": true | false,
  ""pageNumber"": 1
}

2. BuyTicket:
{
  ""intent"": ""BuyTicket"",
  ""flightNumber"": ""FL-xxxx"",
  ""date"": ""YYYY-MM-DDTHH:MM:SS"",
  ""passengerNames"": [""NAME1"", ""NAME2""]
}

3. CheckIn:
{
  ""intent"": ""CheckIn"",
  ""flightNumber"": ""FL-xxxx"",
  ""date"": ""YYYY-MM-DDTHH:MM:SS"",
  ""passengerName"": ""NAME""
}

4. QueryPassengerList:
{
  ""intent"": ""QueryPassengerList"",
  ""flightNumber"": ""FL-xxxx"",
  ""date"": ""YYYY-MM-DDTHH:MM:SS"",
  ""pageNumber"": NUMBER
}

Rules:
- flightNumber must always start with 'FL-' and contain 4 digits (e.g., FL-0010)
- passengerNames should be a list of passenger names mentioned in the user's message.
- passengerNames is for multiple passengers, passengerName is for a single one
- Dates must be in ISO 8601 format: YYYY-MM-DDTHH:MM:SS
- Do not include unnecessary fields. Do not add explanations. Just return a valid JSON.
"
                    },
                    new { role = "user", content = message }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API Error: {response.StatusCode}\n{content}");
            }

            dynamic completion = JsonConvert.DeserializeObject(content);
            string jsonContent = completion.choices[0].message.content.ToString();

            var aiResponse = JsonConvert.DeserializeObject<AiResponseDto>(jsonContent);
            return aiResponse;
            
        }
    }
}
