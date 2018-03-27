using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
    }
}