using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using LinqToDB;

namespace WeatherService.Controllers
{
    public class WeatherStationController : Controller
    {
        [AllowAnonymous]
        [HttpGet("WeatherStation/{id?}")]
        public IActionResult Index(string id)
        {
            using (var db = new WeatherDb())
            {
                var station = db.WeatherStation.FirstOrDefault(s => s.Id.Equals(id));

                if (station == null)
                {
                    return NotFound();
                }

                if (!station.IsPublic && !User.Identity.IsAuthenticated)
                {
                    return Forbid();
                }

                return View(station);
            }
        }

        [AllowAnonymous]
        [HttpGet("WeatherStation/{id?}/Values")]
        public IActionResult Values(string id, string from, string to)
        {
            using (var db = new WeatherDb())
            {
                var station = db.WeatherStation.FirstOrDefault(s => s.Id.Equals(id));

                if (station == null)
                {
                    return NotFound();
                }

                if (!station.IsPublic && !User.Identity.IsAuthenticated)
                {
                    return Forbid();
                }

                DateTime fromDate = DateTime.UtcNow;
                DateTime toDate = DateTime.UtcNow;

                Utils.DateTimeConverter.ParseDateTimeString(from, ref fromDate);
                Utils.DateTimeConverter.ParseDateTimeString(to, ref toDate);

                fromDate = Utils.DateTimeConverter.BeginningOfDay(fromDate);
                toDate = Utils.DateTimeConverter.EndOfDay(toDate);

                var q = from e in db.LogEntry
                        where e.StationId.Equals(id) && e.Timestamp >= fromDate && e.Timestamp <= toDate
                        select e;

                var m = new Dictionary<string, object>();
                var logValues = new List<object[]>();

                foreach (var e in q)
                {
                    logValues.Add(new object[]
                    {
                        e.Timestamp.ToLocalTime(),
                        Math.Round(e.Temperature, 2),
                        e.Pressure,
                        e.Humidity,
                        Math.Round(e.UV, 2) });
                }

                m["aaData"] = logValues;

                return Json(m);
            }
        }
    }
}