using System.Collections.Generic;
using System.Linq;

namespace WeatherService.Models.View
{
    public class SelectedDashboardItem
    {
        public SelectedDashboardItem() { }

        public static SelectedDashboardItem Build(DashboardItem item, Widget widget)
        {
            return new SelectedDashboardItem()
            {
                Widget = widget,
                X = item.X,
                Y = item.Y,
                Stations = item.Filters.Select(s => s.StationId)
            };
        }

        public static SelectedDashboardItem Build(DashboardItemUpdate item, Widget widget)
        {
            return new SelectedDashboardItem()
            {
                Widget = widget,
                X = item.X,
                Y = item.Y,
                Stations = item.Filter
            };
        }

        public Widget Widget { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public IEnumerable<string> Stations { get; set; }
    }
}