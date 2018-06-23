namespace WeatherService.Models.Api
{
    public class LogValue
    {
        public long Timestamp { get; set; }
        public double Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public double UV { get; set; }
    }
}