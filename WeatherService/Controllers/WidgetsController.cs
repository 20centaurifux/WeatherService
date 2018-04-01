using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.Widgets;
using WeatherService.Utils;
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
            var station = GetStation();

            var entry = GetCurrentLogEntry(station);
            var m = CreateSingleStationValue<double>(station);

            if (entry != null)
            {
                m.LastUpdate = entry.Timestamp;
                m.Value = entry.Temperature;
            }

            return View(m);
        }

        [Security.Filters.Widget(Guid = "e4948200-adb3-473d-8dea-9b5291aa566a")]
        public IActionResult Humidity()
        {
            var station = GetStation();

            var entry = GetCurrentLogEntry(station);
            var m = CreateSingleStationValue<int>(station);

            if (entry != null)
            {
                m.LastUpdate = entry.Timestamp;
                m.Value = entry.Humidity;
            }

            return View(m);
        }

        [Security.Filters.Widget(Guid = "8e0ce9be-ce79-4ace-9159-a8a158694fa5")]
        public IActionResult WebcamImage()
        {
            var station = GetStation();
            var gen = new RandomGenerator();

            return View(CreateSingleStationValue<string>(station, DateTime.UtcNow, gen.AppendParameterToUrl(station.WebcamUrl)));
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

        private WeatherStation GetStation()
        {
            return GetStations().First();
        }

        private LogEntry GetCurrentLogEntry(WeatherStation station)
        {
            using (var db = new WeatherDb())
            {
                var entry = db.LogEntry.Where(l => l.StationId.Equals(station.Id)).OrderByDescending(l => l.Timestamp).FirstOrDefault();

                if (entry != null && DateTime.UtcNow.Subtract(entry.Timestamp).TotalHours < 5)
                {
                    return entry;
                }
            }

            return null;
        }

        private static SingleStationValue<T> CreateSingleStationValue<T>(WeatherStation station)
        {
            return new SingleStationValue<T>() { StationName = station.Name, StationLocation = station.Location };
        }

        private static SingleStationValue<T> CreateSingleStationValue<T>(WeatherStation station, DateTime lastUpdate, T value)
        {
            return new SingleStationValue<T>()
            {
                StationName = station.Name,
                StationLocation = station.Location,
                LastUpdate = lastUpdate,
                Value = value
            };
        }
    }
}