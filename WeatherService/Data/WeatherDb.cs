using LinqToDB;
using LinqToDB.Data;
using WeatherService.Models;

namespace WeatherService.Data
{
    public class WeatherDb : DataConnection
    {
        public WeatherDb() : base("WeatherData") { }

        public ITable<MetaInfo> MetaInfo => GetTable<MetaInfo>();
        public ITable<User> User => GetTable<User>();
        public ITable<UserInRole> UserInRole => GetTable<UserInRole>().LoadWith<UserInRole>(m => m.User).LoadWith<UserInRole>(m => m.Role);
        public ITable<UserRole> UserRole => GetTable<UserRole>();
        public ITable<WeatherStation> WeatherStation => GetTable<WeatherStation>();
    }
}