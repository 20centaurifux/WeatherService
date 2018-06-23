using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WeatherService.Data;

namespace WeatherService.Security.ApiAuthentication
{
    public class Middleware
    {
        readonly RequestDelegate _next;
        readonly ApiOptions _options;

        public Middleware(RequestDelegate next, IOptions<ApiOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context, RequestData apiAuthenticationData)
        {
            var path = context.Request.Path.ToString();

            try
            {
                if (IsAPIPath(context))
                {
                    var data = ExtractAuthenticationData(context);

                    if (ValidateRequest(data))
                    {
                        apiAuthenticationData.StationId = data.StationId;
                        apiAuthenticationData.Timestamp = data.Timestamp;
                        apiAuthenticationData.HMAC = data.HMAC;

                        await _next(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Forbidden");
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            catch
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad Request");
            }
        }

        bool IsAPIPath(HttpContext context) => context.Request.Path.ToString().ToLower().StartsWith("/api");

        RequestData ExtractAuthenticationData(HttpContext context)
        {
            var headers = context.Request.Headers;

            return new RequestData()
            {
                StationId = headers["X-WeatherStation-SenderId"],
                Timestamp = Convert.ToInt64(headers["X-WeatherStation-Timestamp"]),
                HMAC = headers["X-WeatherStation-HMAC"]
            };
        }

        bool ValidateRequest(RequestData data)
        {
            var requestTimestamp = Utils.DateTimeConverter.UnixTimestampToDateTime(data.Timestamp);
            var success = false;

            var diff = DateTime.UtcNow.Subtract(requestTimestamp);

            if (diff.TotalSeconds < _options.Timeout && diff.TotalSeconds > 0)
            {
                using (var db = new WeatherDb())
                {
                    var station = db.WeatherStation.First(s => s.Id.Equals(data.StationId));

                    if (!String.IsNullOrEmpty(station.Secret))
                    {
                        var secret = Encoding.ASCII.GetBytes(station.Secret);
                        var hmac = new HMACSHA1(secret);
                        var timestampBytes = Encoding.ASCII.GetBytes(data.Timestamp.ToString());

                        var hashBytes = hmac.ComputeHash(timestampBytes);
                        var hashString = hashBytes.ToHexString();

                        success = hashString.Equals(data.HMAC);
                    }
                }
            }

            return success;
        }
    }
}