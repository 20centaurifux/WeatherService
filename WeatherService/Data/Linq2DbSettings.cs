using IniParser.Model;
using LinqToDB.Configuration;
using System.Collections.Generic;

namespace WeatherService.Data
{
    public class Linq2Dbsettings : ILinqToDBSettings
    {
        public static readonly string DEFAULT_CONFIGURATION = "WeatherService";

        private readonly ConnectionStringSettings[] _settings;

        public Linq2Dbsettings(KeyDataCollection config)
        {
            _settings = new ConnectionStringSettings[]
            {
                new ConnectionStringSettings()
                {
                    Name = DEFAULT_CONFIGURATION,
                    ProviderName = config["Provider"],
                    ConnectionString = config["ConnectionString"]
                }
            };
        }

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                return _settings;
            }
        }

        public IEnumerable<IDataProviderSettings> DataProviders
        {
            get
            {
                yield break;
            }
        }

        public string DefaultConfiguration
        {
            get
            {
                return DEFAULT_CONFIGURATION;
            }
        }

        public string DefaultDataProvider
        {
            get
            {
                return _settings[0].ProviderName;
            }
        }
    }
}