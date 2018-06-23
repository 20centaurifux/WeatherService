using LinqToDB.Mapping;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WeatherService.Models
{
    [Table(Name = "DashboardItem")]
    public class DashboardItem
    {
        [PrimaryKey]
        [Identity]
        [Column(Name = "Id")]
        public int Id { get; set; }

        [StringLength(36)]
        [Column(Name = "UserId")]
        public string UserId { get; set; }

        [StringLength(36)]
        [Column(Name = "WidgetId")]
        public string WidgetId { get; set; }

        [Column(Name = "X")]
        public int X { get; set; }

        [Column(Name = "Y")]
        public int Y { get; set; }

        [LinqToDB.Mapping.Association(ThisKey = "Id", OtherKey = "DashboardItemId")]
        public IEnumerable<DashboardFilter> Filters { get; set; }
    }
}