using System;

namespace WeatherService.Models.Widgets
{
    public abstract class ASingleStation
    {
        public string StationName { get; set; }
        public string StationLocation { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}