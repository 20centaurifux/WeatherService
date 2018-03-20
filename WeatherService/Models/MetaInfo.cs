using LinqToDB.Mapping;

namespace WeatherService.Models
{
    [Table(Name = "MetaInfo")]
    public class MetaInfo
    {
        [Column(Name = "DBRevision")]
        public int DBRevision { get; set; }
    }
}