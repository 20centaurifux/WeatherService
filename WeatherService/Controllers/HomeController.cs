using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WeatherService.Data;
using WeatherService.Models;

namespace WeatherService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            var p = WidgetProvider.FromHostingEnvironment(_env);

            var m = new Dashboard()
            {
                AvailableWidgets = p.LoadWidgets(),
                SupportedStations = new Dictionary<System.Guid, System.Collections.Generic.IEnumerable<WeatherStation>>()
            };

            foreach(var w in m.AvailableWidgets)
            {
                if(User.Identity.IsAuthenticated)
                {
                    m.SupportedStations.Add(w.Guid, p.GetSupportedStations(w));
                }
                else
                {
                    m.SupportedStations.Add(w.Guid, p.GetSupportedPublicStations(w));
                }
            }

            return View(m);
        }
    }
}