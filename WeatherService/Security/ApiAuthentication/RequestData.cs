namespace WeatherService.Security.ApiAuthentication
{
    public class RequestData
    {
        public string StationId { get; set; }
        public long Timestamp { get; set; }
        public string HMAC { get; set; }
    }
}