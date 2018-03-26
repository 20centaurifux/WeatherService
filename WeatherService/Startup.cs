using System.IO;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WeatherService.Models;
using WeatherService.Data;
using IniParser;
using WeatherService.Security.ApiAuthentication;

namespace WeatherService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, UserRole>();
            services.AddTransient<IUserStore<User>, UserStore<User>>();
            services.AddTransient<IRoleStore<UserRole>, UserRoleStore<UserRole>>();
            services.AddScoped<RequestData>();
            services.AddMvc(options => options.MaxModelValidationErrors = 1);
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.AccessDeniedPath = "/Error/AccessDenied";
            });
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                DataConnection.TurnTraceSwitchOn();
                DataConnection.WriteTraceLine = (s1, s2) =>
                {
                    System.Diagnostics.Debug.WriteLine(s1, s2);
                };
            }

            var configPath = Path.Combine(env.ContentRootPath, "WeatherService.ini");
            var parser = new FileIniDataParser();
            var config = parser.ReadFile(configPath);

            DataConnection.DefaultSettings = new Data.Linq2Dbsettings(config["Database"]);

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseApiAuthentication(ApiOptions.FromConfig(config["API"]));
            app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
            app.UseMvcWithDefaultRoute();
        }
    }
}