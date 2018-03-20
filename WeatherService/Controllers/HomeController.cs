using Microsoft.AspNetCore.Mvc;

namespace WeatherService.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
