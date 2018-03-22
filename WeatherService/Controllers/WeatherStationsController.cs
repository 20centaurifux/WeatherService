using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WeatherService.Data;
using LinqToDB;

namespace WeatherService.Controllers
{
    public class WeatherStationsController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            using (var db = new WeatherDb())
            {
                var q = from s in db.WeatherStation
                        select s;

                return View(q.ToArray());
            }
        }

        [HttpDelete("WeatherStations/{id?}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(string id)
        {
            using (var db = new WeatherDb())
            {
                db.WeatherStation
                    .Where(s => s.Id.Equals(id))
                    .Delete();

                return Ok();
            }
        }

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