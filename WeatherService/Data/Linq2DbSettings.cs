using LinqToDB.Configuration;
using System.Collections.Generic;

namespace WeatherService.Data
{
    public class Linq2Dbsettings : ILinqToDBSettings
    {
        private static IConnectionStringSettings[] CONNECTION_STRINGS = new IConnectionStringSettings[]
        {
            new ConnectionStringSettings()
            {
                Name = "WeatherData",
                ProviderName = "Firebird",
                ConnectionString = @"User=SYSDBA;Password=masterkey;Database=WeatherData;Datasource=localhost;"
            }
        };

        public IEnumerable<IDataProviderSettings> DataProviders => null;

        public string DefaultConfiguration => "WeatherData";
        public string DefaultDataProvider => "Firebird";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                return CONNECTION_STRINGS;
            }
        }
    }
}
