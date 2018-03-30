namespace WeatherService.Models.View
{
    public class PublicStationData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public bool IsPublic { get; set; }

        public bool HasTemperature { get; set; }

        public bool HasPressure { get; set; }

        public bool HasHumidity { get; set; }

        public bool HasUV { get; set; }
    }
}