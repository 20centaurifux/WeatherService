using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.Api;
using WeatherService.Security.ApiAuthentication;
using LinqToDB;

namespace WeatherService.Controllers
{
    [Produces("application/json")]
    [Route("api/WeatherLog/{id}")]
    public class WeatherLog : Controller
    {
        private RequestData _apiAuthenticationData;

        public WeatherLog(RequestData apiAuthenticationData)
        {
            _apiAuthenticationData = apiAuthenticationData;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index(string id, [FromBody]IEnumerable<LogValue> values)
        {
            if(!_apiAuthenticationData.StationId.Equals(id))
            {
                return Unauthorized();
            }

            using (var db = new WeatherDb())
            {
                try
                {
                    db.BeginTransaction();

                    foreach(var v in values)
                    {
                        var entry = LogEntry.FromLogValue(v);

                        entry.StationId = id;

                        var existing = db.LogEntry.FirstOrDefault(e => e.StationId.Equals(entry.StationId) && e.Timestamp.Equals(entry.Timestamp));

                        if(existing == null)
                        {
                            db.Insert(entry);
                        }
                        else
                        {
                            entry.Id = existing.Id;

                            db.Update(entry);
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