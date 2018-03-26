using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace WeatherService.Security.ApiAuthentication
{
    public static class Extensions
    {
        public static IApplicationBuilder UseApiAuthentication(this IApplicationBuilder app, ApiOptions opts)
        {
            return app.UseMiddleware<Middleware>(Options.Create(opts));
        }
    }
}