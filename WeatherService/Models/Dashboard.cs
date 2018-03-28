using System;
using System.Collections.Generic;

namespace WeatherService.Models
{
    public class Dashboard
    {
        public IEnumerable<Widget> AvailableWidgets { get; set; }
        public Dictionary<Guid, IEnumerable<WeatherStation>> SupportedStations { get; set; }
    }
}