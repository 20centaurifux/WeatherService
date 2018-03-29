using Microsoft.AspNetCore.Mvc;
using WeatherService.Security.Filters;

namespace WeatherService.Controllers
{
    public class WidgetsController : Controller
    {
        [Widget(Guid = "c72fe6ee-4f94-4732-be36-34b2a1f8f370")]
        public IActionResult Temperature()
        {
            return View();
        }
    }
}