﻿using System.IO;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WeatherService.Models;
using WeatherService.Data;
using WeatherService.Security.ApiAuthentication;
using IniParser;

namespace WeatherService
{
    public class Startup
    {
        IHostingEnvironment _hostingEnv;

        public Startup(IHostingEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var widgetsPath = Path.Combine(_hostingEnv.ContentRootPath, "configuration", "widgets.json");

            services.AddIdentity<User, UserRole>();
            services.AddTransient<IUserStore<User>, UserStore<User>>();
            services.AddTransient<IRoleStore<UserRole>, UserRoleStore<UserRole>>();
            services.AddScoped<RequestData>();
            services.AddScoped<Security.Filters.Widget>();
            services.AddSingleton<WidgetProvider>(new WidgetProvider(widgetsPath));

            services.AddMvc(options =>
            {
                options.MaxModelValidationErrors = 1;
                options.Filters.Add(new Security.Filters.Widget());
            });

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

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

            var configPath = Path.Combine(env.ContentRootPath, "configuration", "WeatherService.ini");
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