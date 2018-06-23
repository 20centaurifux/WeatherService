using System;
using System.ComponentModel.DataAnnotations;
using LinqToDB.Mapping;
using WeatherService.Models.View;

namespace WeatherService.Models
{
    [Table(Name = "WeatherStation")]
    public class WeatherStation
    {
        public WeatherStation() => Id = Guid.NewGuid().ToString();

        [PrimaryKey]
        [StringLength(36)]
        [Column(Name = "Id")]
        public string Id { get; set; }

        [Required]
        [StringLength(64)]
        [Column(Name = "Name")]
        public string Name { get; set; }

        [StringLength(64)]
        [Column(Name = "Secret")]
        public string Secret { get; set; }

        [Column(Name = "Location")]
        [StringLength(64)]
        public string Location { get; set; }

        [Column(Name = "WebcamUrl")]
        [StringLength(256)]
        public string WebcamUrl { get; set; }

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

        public PublicStationData ToPublicStationData()
        {
            return new PublicStationData()
            {
                Id = this.Id,
                Name = this.Name,
                Location = this.Location,
                WebcamUrl = this.WebcamUrl,
                IsPublic = this.IsPublic,
                HasTemperature = this.HasTemperature,
                HasHumidity = this.HasHumidity,
                HasPressure = this.HasPressure,
                HasUV = this.HasUV
            };
        }
    }
}