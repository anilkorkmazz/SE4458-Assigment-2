using Microsoft.AspNetCore.Mvc;
using backend_gateway.DTOs;
using backend_gateway.Services;
using Newtonsoft.Json;

namespace backend_gateway.Controllers
{
    [ApiController]
    [Route("api/agent")]
    public class ChatGatewayController : ControllerBase
    {
        private readonly OpenAiService _openAiService;
        private readonly AirlineApiService _airlineApiService;

        public ChatGatewayController(OpenAiService openAiService, AirlineApiService airlineApiService)
        {
            _openAiService = openAiService;
            _airlineApiService = airlineApiService;
        }

        [HttpPost("message")]
        public async Task<IActionResult> HandleMessage([FromBody] ChatRequestDto dto)
        {
            try
            {
                
                var aiResult = await _openAiService.GetIntentAsync(dto.Message);

                Console.WriteLine("🎯 Intent & Parameters:");
                Console.WriteLine(JsonConvert.SerializeObject(aiResult, Formatting.Indented));

                
                var response = await _airlineApiService.CallAirlineApiAsync(aiResult);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine("✈️ Airline API Response:");
                Console.WriteLine(content);

               
                var parsedJson = JsonConvert.DeserializeObject<object>(content);
                return Ok(parsedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
