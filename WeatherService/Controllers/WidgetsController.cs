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

            var m = new SingleStationValue<Double>()
            {
                StationName = station.Name,
                StationLocation = station.Location
            };

            using (var db = new WeatherDb())
            {
                var entry = db.LogEntry.Where(l => l.StationId.Equals(station.Id)).OrderByDescending(l => l.Timestamp).FirstOrDefault();

                if(entry != null && DateTime.UtcNow.Subtract(entry.Timestamp).TotalHours < 4)
                {
                    m.LastUpdate = entry.Timestamp;
                    m.Value = entry.Temperature;
                }
            }

            return View(m);
        }

        [Security.Filters.Widget(Guid = "8e0ce9be-ce79-4ace-9159-a8a158694fa5")]
        public IActionResult WebcamImage()
        {
            var station = GetStations().First();

            var m = new SingleStationValue<String>()
            {
                StationName = station.Name,
                StationLocation = station.Location,
                Value = station.WebcamUrl
            };

            return View(m);
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