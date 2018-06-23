using System;
using IniParser.Model;

namespace WeatherService.Security.ApiAuthentication
{
    public class ApiOptions
    {
        public ApiOptions() => Timeout = 15;

        public static ApiOptions FromConfig(KeyDataCollection config)
        {
            var options = new ApiOptions();

            try
            {
                if (config.ContainsKey("Timeout"))
                {
                    options.Timeout = Convert.ToInt32(config["Timeout"]);
                }
            }
            catch { }

            return options;
        }

        public int Timeout { get; set; }
    }
}