using System;

namespace WeatherService.Models
{
    public class Widget
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; }
        public bool SupportsMultipleStations{ get; set; }
        public bool RequiresTemperature { get; set; }
        public bool RequiresPressure { get; set; }
        public bool RequiresHumidity { get; set; }
        public bool RequiresUV { get; set; }
        public bool RequiresWebcamUrl { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Background { get; set; }
    }
}