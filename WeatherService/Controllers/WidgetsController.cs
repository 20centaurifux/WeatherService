using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.Widgets;
using WeatherService.Utils;

namespace WeatherService.Controllers
{
    public class WidgetsController : Controller
    {
        public static readonly int CURRENT_LOG_ENTRY_HOURS_FILTER = 12;

        [Security.Filters.Widget(Guid = "c72fe6ee-4f94-4732-be36-34b2a1f8f370")]
        public IActionResult Temperature()
        {
            var station = GetStation();

            var entry = GetCurrentLogEntry(station);
            var m = CreateSingleStationValue<Double?>(station);

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
            var m = CreateSingleStationValue<int?>(station);

            if (entry != null)
            {
                m.LastUpdate = entry.Timestamp;
                m.Value = entry.Humidity;
            }

            return View(m);
        }

        [Security.Filters.Widget(Guid = "15fe2e9c-53b6-44e4-afb2-1d3b417b93d5")]
        public IActionResult Pressure()
        {
            var station = GetStation();

            using (var db = new WeatherDb())
            {
                var start = Utils.DateTimeConverter.BeginningOfDay(DateTime.UtcNow);
                var end = Utils.DateTimeConverter.EndOfDay(start);

                var m = new LogEntries()
                {
                    StationName = station.Name,
                    Entries = db.LogEntry.Where(l => l.StationId.Equals(station.Id) && l.Timestamp >= start && l.Timestamp <= end).ToArray()
                };

                return View(m);
            };
        }

        [Security.Filters.Widget(Guid = "c21e0735-d07c-4977-8d4a-0dbf5d899e6a")]
        public IActionResult WeeklyOverview()
        {
            var station = GetStation();

            using (var db = new WeatherDb())
            {
                var end = DateTime.UtcNow;
                var start = Utils.DateTimeConverter.BeginningOfDay(end.AddDays(-6));

                var m = new LogEntries()
                {
                    StationName = station.Name,
                    Entries = db.LogEntry.Where(l => l.StationId.Equals(station.Id) && l.Timestamp >= start && l.Timestamp <= end).ToArray()
                };

                return View(m);
            };
        }

        [Security.Filters.Widget(Guid = "8e0ce9be-ce79-4ace-9159-a8a158694fa5")]
        public IActionResult WebcamImage()
        {
            var station = GetStation();
            var gen = new RandomGenerator();
            var url = gen.AppendParameterToUrl(station.WebcamUrl);

            var m = CreateSingleStationValue<string>(station, DateTime.UtcNow, url);

            return View(m);
        }

        IEnumerable<WeatherStation> GetStations()
        {
            using (var db = new WeatherDb())
            {
                var stationIds = Request.Query["s"].ToString().Split(",");
                Func<string, WeatherStation> f = id => db.WeatherStation.First(s => s.Id.Equals(id));

                return stationIds.Select(f).ToArray();
            }
        }

        WeatherStation GetStation()
        {
            return GetStations().First();
        }

        LogEntry GetCurrentLogEntry(WeatherStation station)
        {
            using (var db = new WeatherDb())
            {
                var entry = db.LogEntry.Where(l => l.StationId.Equals(station.Id)).OrderByDescending(l => l.Timestamp).FirstOrDefault();

                if (entry != null && DateTime.UtcNow.Subtract(entry.Timestamp).TotalHours <= CURRENT_LOG_ENTRY_HOURS_FILTER)
                {
                    return entry;
                }
            }

            return null;
        }

        static SingleStationValue<T> CreateSingleStationValue<T>(WeatherStation station)
        {
            return new SingleStationValue<T>() { StationName = station.Name, StationLocation = station.Location };
        }

        static SingleStationValue<T> CreateSingleStationValue<T>(WeatherStation station, DateTime lastUpdate, T value)
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