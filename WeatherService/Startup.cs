using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WeatherService.Models;
using WeatherService.Data;
using Microsoft.AspNetCore.Http;

namespace WeatherService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, UserRole>();
            services.AddTransient<IUserStore<User>, UserStore<User>>();
            services.AddTransient<IRoleStore<UserRole>, UserRoleStore<UserRole>>();
            services.AddMvc(options => options.MaxModelValidationErrors = 1);
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Login");
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                DataConnection.DefaultSettings = new Data.Linq2Dbsettings();
                DataConnection.TurnTraceSwitchOn();
                DataConnection.WriteTraceLine = (s1, s2) =>
                {
                    System.Diagnostics.Debug.WriteLine(s1, s2);
                };
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}