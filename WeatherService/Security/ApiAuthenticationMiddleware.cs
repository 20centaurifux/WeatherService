using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using WeatherService.Data;
using Microsoft.Extensions.Options;

namespace WeatherService.Security
{
    public class ApiAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiAuthenticationOptions _options;

        public ApiAuthenticationMiddleware(RequestDelegate next, IOptions<ApiAuthenticationOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context, ApiAuthenticationRequestData apiAuthenticationData)
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

        private bool IsAPIPath(HttpContext context)
        {
            return context.Request.Path.ToString().ToLower().StartsWith("/api");
        }

        private ApiAuthenticationRequestData ExtractAuthenticationData(HttpContext context)
        {
            var headers = context.Request.Headers;

            return new ApiAuthenticationRequestData()
            {
                StationId = headers["X-WeatherStation-SenderId"],
                Timestamp = Convert.ToInt64(headers["X-WeatherStation-Timestamp"]),
                HMAC = headers["X-WeatherStation-HMAC"]
            };
        }

        private bool ValidateRequest(ApiAuthenticationRequestData data)
        {
            var requestTimestamp = Utils.DateTimeConverter.UnixTimestampToDateTime(data.Timestamp);
            var success = false;

            var diff = DateTime.UtcNow.Subtract(requestTimestamp);

            if (diff.TotalSeconds < _options.Timeout && diff.TotalSeconds > 0)
            {
                using (var db = new WeatherDb())
                {
                    var station = db.WeatherStation.First(s => s.Id.Equals(data.StationId));

                    var secret = Encoding.ASCII.GetBytes(station.Secret);
                    var hmac = new HMACSHA1(secret);
                    var timestampBytes = Encoding.ASCII.GetBytes(data.Timestamp.ToString());

                    var hashBytes = hmac.ComputeHash(timestampBytes);
                    var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                    success = hashString.Equals(data.HMAC);
                }
            }

            return success;
        }
    }
}