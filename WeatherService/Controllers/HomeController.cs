using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WeatherService.Data;
using WeatherService.Models;

namespace WeatherService.Controllers
{
    public class HomeController : Controller
    {
        private readonly WidgetProvider _widgetProvider;

        public HomeController(WidgetProvider widgetProvider)
        {
            _widgetProvider = widgetProvider;
        }

        public IActionResult Index()
        {
            var m = new Dashboard()
            {
                AvailableWidgets = _widgetProvider.LoadWidgets(),
                SupportedStations = new Dictionary<System.Guid, System.Collections.Generic.IEnumerable<WeatherStation>>()
            };

            foreach(var w in m.AvailableWidgets)
            {
                if(User.Identity.IsAuthenticated)
                {
                    m.SupportedStations.Add(w.Guid, _widgetProvider.GetSupportedStations(w));
                }
                else
                {
                    m.SupportedStations.Add(w.Guid, _widgetProvider.GetSupportedPublicStations(w));
                }
            }

            return View(m);
        }
    }
}