namespace backend_gateway.DTOs
{
    public class ChatRequestDto
    {
        public string Message { get; set; }
    }

    public class AiResponseDto
    {
        public string Intent { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string AirportFrom { get; set; }
        public string AirportTo { get; set; }
        public int? NumberOfPeople { get; set; }
        public bool? IsRoundTrip { get; set; }
        public int? PageNumber { get; set; }

        public string FlightNumber { get; set; }
        public string Date { get; set; }
        public List<string> PassengerNames { get; set; }
        public string PassengerName { get; set; }
    }
}