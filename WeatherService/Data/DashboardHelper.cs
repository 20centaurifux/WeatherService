using System.Linq;
using LinqToDB;

namespace WeatherService.Data
{
    public static class DashboardHelper
    {
        public static void DeleteDashboardItems(WeatherDb db, string userId)
        {
            if (db.DataProvider.Name.Equals("SQLite"))
            {
                var ids = from item in db.DashboardItem
                          where item.UserId.Equals(userId)
                          select item.Id;

                db.DashbordFilter
                    .Where(f => ids.Contains(f.DashboardItemId))
                    .Delete();

                db.DashboardItem
                    .Where(item => ids.Contains(item.Id))
                    .Delete();
            }
            else
            {
                db.DashboardItem
                    .Where(item => item.UserId.Equals(userId))
                    .Delete();
            }
        }
    }
}