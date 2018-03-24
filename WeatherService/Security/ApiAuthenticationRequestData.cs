namespace WeatherService.Security
{
    public class ApiAuthenticationRequestData
    {
        public string StationId { get; set; }
        public long Timestamp { get; set; }
        public string HMAC { get; set; }
    }
}