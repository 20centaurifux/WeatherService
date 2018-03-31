using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.Widgets;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WeatherService.Controllers
{
    public class WidgetsController : Controller
    {
        [Security.Filters.Widget(Guid = "c72fe6ee-4f94-4732-be36-34b2a1f8f370")]
        public IActionResult Temperature()
        {
            var station = GetStations().First();
            DateTime? lastUpdate = null;
            Double? value = null;

            using (var db = new WeatherDb())
            {
                var entry = db.LogEntry.Where(l => l.StationId.Equals(station.Id)).OrderByDescending(l => l.Timestamp).FirstOrDefault();

                if(entry != null && DateTime.UtcNow.Subtract(entry.Timestamp).TotalHours < 3)
                {
                    lastUpdate = entry.Timestamp;
                    value = entry.Temperature;
                }
            }

            return View(new Temperature() { StationName = station.Name, StationLocation = station.Location, LastUpdate = lastUpdate, Value = value });
        }

        private IEnumerable<WeatherStation> GetStations()
        {
            using (var db = new WeatherDb())
            {
                var stationIds = Request.Query["s"].ToString().Split(",");
                Func<string, WeatherStation> f = id => db.WeatherStation.First(s => s.Id.Equals(id));

                return stationIds.Select(f).ToArray();
            }
        }
    }
}