using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using WeatherService.Models;
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
                        select s.ToPublicStationData();

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
                using (var db = new WeatherDb())
                {
                    db.BeginTransaction();

                    try
                    {
                        if (db.WeatherStation.Any(s => s.Name.EqualsICase(m.Name) && !s.Id.Equals(m.Id)))
                        {
                            ViewData["ValidationError"] = "A weather station with the given name does already exist.";
                        }
                        else
                        {
                            db.Update(m);
                            db.CommitTransaction();

                            return Redirect("/WeatherStations");
                        }
                    }
                    catch(Exception ex)
                    {
                        db.RollbackTransaction();
                        throw ex;
                    }
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
                using (var db = new WeatherDb())
                {
                    db.BeginTransaction();

                    try
                    {
                        if(db.WeatherStation.Any(s => s.Name.EqualsICase(m.Name)))
                        {
                            ViewData["ValidationError"] = "A weather station with the given name does already exist.";
                        }
                        else
                        {
                            db.Insert(m);
                            db.CommitTransaction();

                            return Redirect("/WeatherStations");
                        }
                    }
                    catch(Exception ex)
                    {
                        db.RollbackTransaction();
                        throw ex;
                    }
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
                db.BeginTransaction();

                try
                {
                    if (db.DataProvider.Name.Equals("SQLite"))
                    {
                        db.LogEntry
                            .Where(e => e.StationId.Equals(id))
                            .Delete();

                        db.DashbordFilter
                            .Where(f => f.StationId.Equals(id))
                            .Delete();
                    }

                    var q = from item in db.DashboardItem
                            join filter in db.DashbordFilter on item.Id equals filter.DashboardItemId into filters
                            where filters.Count() == 0
                            select item;

                    q.Delete();

                    db.WeatherStation
                        .Where(s => s.Id.Equals(id))
                        .Delete();

                    db.CommitTransaction();
                }
                catch(Exception ex)
                {
                    db.RollbackTransaction();
                    throw ex;
                }

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

                var stations = q.Select(s => s.ToPublicStationData());

                return Json(stations.ToArray());
            }
        }
    }
}