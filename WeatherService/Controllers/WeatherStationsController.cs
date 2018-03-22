using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WeatherService.Data;

namespace WeatherService.Controllers
{
    public class WeatherStationsController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Available()
        {
            using (var db = new WeatherDb())
            {
                var q = from s in db.WeatherStation
                        select s;

                if(!User.Identity.IsAuthenticated)
                {
                    q = q.Where(s => s.IsPublic);
                }

                return Json(q.ToArray());
            }
        }
    }
}