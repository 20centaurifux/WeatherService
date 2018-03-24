using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace WeatherService.Security
{
    public static class ApiAuthenticationExtensions
    {
        public static IApplicationBuilder UseApiAuthentication(this IApplicationBuilder app, ApiAuthenticationOptions opts)
        {
            return app.UseMiddleware<ApiAuthenticationMiddleware>(Options.Create(opts));
        }
    }
}