using System;
using LinqToDB.Mapping;

namespace WeatherService.Models
{
    [Table(Name = "LogEntry")]
    public class LogEntry
    {
        [PrimaryKey]
        [Identity]
        [Column(Name = "Id")]
        public int Id { get; set; }

        [Column(Name = "StationId")]
        public string StationId { get; set; }

        [Column(Name = "LogDate")]
        public DateTime Timestamp { get; set; }

        [Column(Name = "Temperature")]
        public double Temperature { get; set; }

        [Column(Name = "Pressure")]
        public int Pressure { get; set; }

        [Column(Name = "Humidity")]
        public int Humidity { get; set; }

        [Column(Name = "UV")]
        public double UV { get; set; }

        public static LogEntry FromLogValue(Api.LogValue value)
        {
            return new LogEntry()
            {
                Timestamp = Utils.DateTimeConverter.UnixTimestampToDateTime(value.Timestamp),
                Temperature = value.Temperature,
                Pressure = value.Pressure,
                Humidity = value.Humidity,
                UV = value.UV
            };
        }
    }
}