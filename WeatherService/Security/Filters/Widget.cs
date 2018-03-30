using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using WeatherService.Data;
using WeatherService.Models;
using Microsoft.AspNetCore.Mvc;

namespace WeatherService.Security.Filters
{
    public class Widget : Attribute, IActionFilter
    {
        private ActionExecutingContext _context;
        private WidgetProvider _widgetProvider;

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

        private void Prepare(ActionExecutingContext context)
        {
            _context = context;
            _widgetProvider = (WidgetProvider)_context.HttpContext.RequestServices.GetService(typeof(WidgetProvider));
        }

        private void Process()
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

        private IEnumerable<string> GetRequestedStationIds()
        {
            return _context.HttpContext.Request.Query["s"].ToString().Split(",");
        }

        private IEnumerable<WeatherStation> GetSupportedStations(Models.Widget widget)
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

        private void BadRequest()
        {
            _context.Result = new BadRequestResult();
        }

        private void Forbid()
        {
            _context.Result = new ForbidResult();
        }
    }
}