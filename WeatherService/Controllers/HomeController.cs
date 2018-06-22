using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LinqToDB;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.View;

namespace WeatherService.Controllers
{
    public class HomeController : Controller
    {
        readonly WidgetProvider _widgetProvider;
        readonly UserManager<User> _userManager;

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

            foreach (var widget in m.AvailableWidgets)
            {
                if (User.Identity.IsAuthenticated)
                {
                    m.SupportedStations.Add(widget.Guid, _widgetProvider.GetSupportedStations(widget).Select(s => s.ToPublicStationData()));
                }
                else
                {
                    m.SupportedStations.Add(widget.Guid, _widgetProvider.GetSupportedPublicStations(widget).Select(s => s.ToPublicStationData()));
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                using (var db = new WeatherDb())
                {
                    LoadDashboardFromDatabase(db, ref m);
                }
            }
            else
            {
                LoadDashboardFromSession(ref m);
            }

            return View(m);
        }

        void LoadDashboardFromDatabase(WeatherDb db, ref Dashboard dashboard)
        {
            var selectedItems = new List<SelectedDashboardItem>();
            var user = _userManager.GetUserAsync(User).Result;
            var dashboardItems = db.DashboardItem.Where(item => item.UserId.Equals(user.Id));

            foreach (var dashboardItem in dashboardItems)
            {
                var widget = dashboard.AvailableWidgets.FirstOrDefault(w => w.Guid.Equals(new Guid(dashboardItem.WidgetId)));

                if (widget != null)
                {
                    if (_widgetProvider.ValidateStationIds(widget, dashboardItem.Filters.Select(f => f.StationId)))
                    {
                        selectedItems.Add(SelectedDashboardItem.Build(dashboardItem, widget));
                    }
                }
            }

            dashboard.Items = selectedItems;
        }

        void LoadDashboardFromSession(ref Dashboard dashboard)
        {
            var selectedItems = new List<SelectedDashboardItem>();

            if (HttpContext.Session.TryGetValue("Dashboard", out byte[] bytes))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();

                    ms.Write(bytes, 0, bytes.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    var items = (IEnumerable<DashboardItemUpdate>)formatter.Deserialize(ms);

                    foreach (var item in items)
                    {
                        var widget = dashboard.AvailableWidgets.FirstOrDefault(w => w.Guid.Equals(new Guid(item.WidgetId)));

                        if (widget != null)
                        {
                            if (_widgetProvider.ValidatePublicStationIds(widget, item.Filter))
                            {
                                selectedItems.Add(SelectedDashboardItem.Build(item, widget));
                            }
                        }
                    }
                }
            }

            dashboard.Items = selectedItems;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult UpdateDashboard([FromBody] IEnumerable<DashboardItemUpdate> items)
        {
            if (User.Identity.IsAuthenticated)
            {
                StoreDashboardInDatabase(items);
            }
            else
            {
                StoreDashboardInSession(items);
            }

            return Ok();
        }

        void StoreDashboardInDatabase(IEnumerable<DashboardItemUpdate> items)
        {
            var user = _userManager.GetUserAsync(User).Result;

            using (var db = new WeatherDb())
            {
                db.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    DashboardHelper.DeleteDashboardItems(db, user.Id);

                    foreach (var item in items)
                    {
                        var widget = _widgetProvider.LoadWidget(new Guid(item.WidgetId));

                        if (widget != null)
                        {
                            if (_widgetProvider.ValidateStationIds(widget, item.Filter))
                            {
                                var pk = db.InsertWithInt32Identity(new DashboardItem() { UserId = user.Id, WidgetId = item.WidgetId, X = item.X, Y = item.Y });

                                foreach (var stationId in item.Filter)
                                {
                                    db.Insert(new DashboardFilter() { DashboardItemId = pk, StationId = stationId });
                                }
                            }
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();
                    throw ex;
                }
            }
        }

        void StoreDashboardInSession(IEnumerable<DashboardItemUpdate> items)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, items);

                HttpContext.Session.Set("Dashboard", ms.ToArray());
            }
        }
    }
}