using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using WeatherService.Models;

namespace WeatherService.Security.Filters
{
    public class Widget : Attribute, IActionFilter
    {
        ActionExecutingContext _context;
        WidgetProvider _widgetProvider;

        public string Guid { get; set; }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!string.IsNullOrEmpty(Guid))
            {
                Prepare(context);
                Process();
            }
        }

        void Prepare(ActionExecutingContext context)
        {
            _context = context;
            _widgetProvider = (WidgetProvider)_context.HttpContext.RequestServices.GetService(typeof(WidgetProvider));
        }

        void Process()
        {
            var stationIds = GetRequestedStationIds();

            if(stationIds.Count() == 0)
            {
                BadRequest();
                return;
            }

            var widget = _widgetProvider.LoadWidget(new Guid(this.Guid));
            var supportedStations = GetSupportedStations(widget);

            foreach(var stationId in stationIds)
            {
                var station = supportedStations.FirstOrDefault(s => s.Id.Equals(stationId));

                if (station == null)
                {
                    Forbid();
                    return;
                }

                if(!WidgetProvider.WidgetCompatibleToStation(widget, station))
                {
                    BadRequest();
                    return;
                }
            }
        }

        IEnumerable<string> GetRequestedStationIds() => _context.HttpContext.Request.Query["s"].ToString().Split(",");

        IEnumerable<WeatherStation> GetSupportedStations(Models.Widget widget)
        {
            IEnumerable<WeatherStation> supportedStations;

            if (_context.HttpContext.User.Identity.IsAuthenticated)
            {
                supportedStations = _widgetProvider.GetSupportedStations(widget);
            }
            else
            {
                supportedStations = _widgetProvider.GetSupportedPublicStations(widget);
            }

            return supportedStations;
        }

        void BadRequest() => _context.Result = new BadRequestResult();

        void Forbid() => _context.Result = new ForbidResult();
    }
}