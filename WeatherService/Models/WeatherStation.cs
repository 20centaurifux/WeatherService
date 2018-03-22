using Microsoft.AspNetCore.Identity;
using System;
using LinqToDB.Mapping;

namespace WeatherService.Models
{
    [Table(Name = "WeatherStation")]
    public class WeatherStation
    {
        public WeatherStation()
        {
            Id = Guid.NewGuid().ToString();
        }

        [PrimaryKey]
        [Column(Name = "Id")]
        public string Id { get; set; }

        [Column(Name = "Name")]
        public string Name { get; set; }

        [Column(Name = "Location")]
        public string Location { get; set; }

        [Column(Name = "Latitude")]
        public double Latitude { get; set; }

        [Column(Name = "Longitude")]
        public double Longitude { get; set; }

        [Column(Name = "IsPublic")]
        public bool IsPublic { get; set; }

        [Column(Name = "HasTemperature")]
        public bool HasTemperature { get; set; }

        [Column(Name = "HasPressure")]
        public bool HasPressure { get; set; }

        [Column(Name = "HasHumidity")]
        public bool HasHumidity { get; set; }

        [Column(Name = "HasUV")]
        public bool HasUV { get; set; }
    }
}