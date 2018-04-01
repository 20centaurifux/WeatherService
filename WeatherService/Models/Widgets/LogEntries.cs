using System;
using System.Collections.Generic;

namespace WeatherService.Models.Widgets
{
    public class LogEntries
    {
        public string StationName { get; set; }
        public IEnumerable<LogEntry> Entries { get; set; }
    }
}