using LinqToDB.Mapping;
using System.ComponentModel.DataAnnotations;

namespace WeatherService.Models
{
    [Table(Name = "DashboardFilter")]
    public class DashboardFilter
    {
        [Column(Name = "DashboardItemId")]
        public int DashboardItemId { get; set; }

        [StringLength(36)]
        [Column(Name = "WeatherStationId")]
        public string StationId { get; set; }
    }
}