namespace WeatherService.Security
{
    public class ApiAuthenticationOptions
    {
        public ApiAuthenticationOptions()
        {
            Timeout = 15;
        }

        public int Timeout { get; set; }
    }
}