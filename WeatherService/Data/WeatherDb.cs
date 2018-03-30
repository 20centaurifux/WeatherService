using LinqToDB;
using LinqToDB.Data;
using WeatherService.Models;

namespace WeatherService.Data
{
    public class WeatherDb : DataConnection
    {
        public WeatherDb() : base(Linq2Dbsettings.DEFAULT_CONFIGURATION) { }

        public ITable<MetaInfo> MetaInfo => GetTable<MetaInfo>();
        public ITable<User> User => GetTable<User>();
        public ITable<UserInRole> UserInRole => GetTable<UserInRole>().LoadWith<UserInRole>(m => m.User).LoadWith<UserInRole>(m => m.Role);
        public ITable<UserRole> UserRole => GetTable<UserRole>();
        public ITable<WeatherStation> WeatherStation => GetTable<WeatherStation>();
        public ITable<LogEntry> LogEntry => GetTable<LogEntry>();
        public ITable<DashboardItem> DashboardItem => GetTable<DashboardItem>().LoadWith(i => i.Filters);
        public ITable<DashboardFilter> DashbordFilter => GetTable<DashboardFilter>();
    }
}