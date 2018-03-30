using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.View;

namespace WeatherService.Controllers
{
    public class HomeController : Controller
    {
        private readonly WidgetProvider _widgetProvider;
        private readonly UserManager<User> _userManager;

        public HomeController(WidgetProvider widgetProvider, UserManager<User> userManager)
        {
            _widgetProvider = widgetProvider;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var m = new Dashboard()
            {
                AvailableWidgets = _widgetProvider.LoadWidgets(),
                SupportedStations = new Dictionary<System.Guid, IEnumerable<PublicStationData>>()
            };

            foreach(var w in m.AvailableWidgets)
            {
                if(User.Identity.IsAuthenticated)
                {
                    m.SupportedStations.Add(w.Guid, _widgetProvider.GetSupportedStations(w).Select(s => s.ToPublicStationData()));
                }
                else
                {
                    m.SupportedStations.Add(w.Guid, _widgetProvider.GetSupportedPublicStations(w).Select(s => s.ToPublicStationData()));
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserAsync(User).Result;

                using (var db = new WeatherDb())
                {
                    var dashboardItems = db.DashboardItem.Where(i => i.UserId.Equals(user.Id)).ToArray();
                    var items = new List<SelectedDashboardItem>();

                    foreach(var dashboardItem in dashboardItems)
                    {
                        var widget = m.AvailableWidgets.FirstOrDefault(w => w.Guid.Equals(new Guid(dashboardItem.WidgetId)));

                        if (widget != null)
                        {
                            items.Add(SelectedDashboardItem.Build(dashboardItem, widget));
                        }
                    }

                    m.Items = items;
                }
            }
            else
            {
                m.Items = new SelectedDashboardItem[] { };
            }

            return View(m);
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateDashboard([FromBody] IEnumerable<DashboardItemUpdate> items)
        {
            var user = _userManager.GetUserAsync(User).Result;

            using (var db = new WeatherDb())
            {
                db.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    db.DashboardItem
                        .Where(item => item.UserId.Equals(user.Id))
                        .Delete();

                    foreach (var item in items)
                    {
                        var pk = db.InsertWithInt32Identity(new DashboardItem() { UserId = user.Id, WidgetId = item.WidgetId, X = item.X, Y = item.Y });

                        foreach (var stationId in item.Filter)
                        {
                            db.Insert(new DashboardFilter() { DashboardItemId = pk, StationId = stationId });
                        }
                    }

                    db.CommitTransaction();
                }
                catch(Exception ex)
                {
                    db.RollbackTransaction();
                    throw ex;
                }
            }

            return Ok();
        }
    }
}