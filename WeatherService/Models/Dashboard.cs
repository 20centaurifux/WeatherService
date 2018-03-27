using System.Collections.Generic;

namespace WeatherService.Models
{
    public class Dashboard
    {
        public IEnumerable<Widget> AvailableWidgets { get; set; }
    }
}