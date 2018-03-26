using System;
using IniParser.Model;

namespace WeatherService.Security
{
    public class ApiAuthenticationOptions
    {
        public ApiAuthenticationOptions()
        {
            Timeout = 15;
        }

        public static ApiAuthenticationOptions FromConfig(KeyDataCollection config)
        {
            var options = new ApiAuthenticationOptions();

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