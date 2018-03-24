using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.Api;
using LinqToDB;
using Microsoft.AspNetCore.Authorization;

namespace WeatherService.Controllers
{
    [Produces("application/json")]
    [Route("api/WeatherLog/{id}")]
    public class WeatherLog : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index(string id, [FromBody]IEnumerable<LogValue> values)
        {
            using (var transaction = new TransactionScope())
            {
                using (var db = new WeatherDb())
                {
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

                    transaction.Complete();
                }
            }

            return Ok();
        }
    }
}