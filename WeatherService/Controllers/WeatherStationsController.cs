using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WeatherService.Data;
using WeatherService.Models;
using LinqToDB;
using System.Transactions;

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

        [HttpGet("WeatherStations/Edit/{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(string id)
        {
            using (var db = new WeatherDb())
            {
                var m = db.WeatherStation.FirstOrDefault(s => s.Id.Equals(id));

                if(m == null)
                {
                    return NotFound();
                }

                return View("EditStation", m);
            }
        }

        [HttpPost("WeatherStations/Update")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(WeatherStation m)
        {
            if (TryValidateModel(m))
            {
                using (var transaction = new TransactionScope())
                {
                    using (var db = new WeatherDb())
                    {
                        if (db.WeatherStation.Any(s => s.Name.ToLower().Equals(m.Name.ToLower()) && !s.Id.Equals(m.Id)))
                        {
                            ViewData["ValidationError"] = "A weather station with the given name does already exist.";
                        }
                        else
                        {
                            db.Update(m);

                            return Redirect("/WeatherStations");
                        }
                    }

                    transaction.Complete();
                }
            }
            else
            {
                ViewData["ValidationError"] = "Couldn't validate input, please check entered data.";
            }

            return View("EditStation", m);
        }

        [HttpGet("WeatherStations/Create")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View("CreateStation", new WeatherStation() { Name = "New Station" });
        }

        [HttpPost("WeatherStations/Create")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(WeatherStation m)
        {
            if (TryValidateModel(m))
            {
                using (var transaction = new TransactionScope())
                {
                    using (var db = new WeatherDb())
                    {
                        if(db.WeatherStation.Any(s => s.Name.ToLower().Equals(m.Name.ToLower())))
                        {
                            ViewData["ValidationError"] = "A weather station with the given name does already exist.";
                        }
                        else
                        {
                            db.Insert(m);

                            return Redirect("/WeatherStations");
                        }
                    }

                    transaction.Complete();
                }
            }
            else
            {
                ViewData["ValidationError"] = "Couldn't validate input, please check entered data.";
            }

            return View("CreateStation", m);
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