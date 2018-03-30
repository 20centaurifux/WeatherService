using System;
using System.Collections.Generic;
using WeatherService.Models.View;

namespace WeatherService.Models.View
{
    public class Dashboard
    {
        public IEnumerable<Widget> AvailableWidgets { get; set; }
        public Dictionary<Guid, IEnumerable<WeatherStation>> SupportedStations { get; set; }
        public IEnumerable<SelectedDashboardItem> Items { get; set; }
    }
}