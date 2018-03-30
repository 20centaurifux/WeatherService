using System;
using System.Collections.Generic;

namespace WeatherService.Models.View
{
    public class Dashboard
    {
        public IEnumerable<Widget> AvailableWidgets { get; set; }
        public Dictionary<Guid, IEnumerable<PublicStationData>> SupportedStations { get; set; }
        public IEnumerable<SelectedDashboardItem> Items { get; set; }
    }
}