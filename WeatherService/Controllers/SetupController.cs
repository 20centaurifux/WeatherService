using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Data;
using WeatherService.Models;
using LinqToDB;

namespace WeatherService.Controllers
{
    public class SetupController : Controller
    {
        RoleManager<UserRole> _roleManager;
        UserManager<User> _userManager;

        public SetupController(RoleManager<UserRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var message = "Upgrade not required.";

            using (var db = new WeatherDb())
            {
                var info = db.MetaInfo.FirstOrDefault();

                var gen = new PasswordGenerator(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true, passwordLength: 8);
                var password = gen.Next();

                if (info == null)
                {
                    var adminRole = _roleManager.FindByNameAsync("Adminstrator").Result;

                    if(adminRole == null)
                    {
                        adminRole = new UserRole() { Name = "Administrator" };

                        _roleManager.CreateAsync(adminRole);
                    }

                    if (!_roleManager.RoleExistsAsync("Reader").Result)
                    {
                        _roleManager.CreateAsync(new UserRole() { Name = "Reader" });
                    }

                    var admin = new User() { UserName = "admin" };

                    var result = _userManager.CreateAsync(admin, password).Result;

                    if(result.Succeeded)
                    {
                        info = new MetaInfo() { DBRevision = 1 };
                        db.Insert(info);

                        message = string.Format("Upgraded to database revision 1. \"Admin\" account with password \"{0}\" created.", password);
                    }
                    else if(result.Errors.Count() > 0)
                    {
                        message = string.Join(" ", result.Errors.Select(e => e.Description));
                    }
                    else
                    {
                        message = "Couldn't create admin account.";
                    }
                }
            }

            return Content(message);
        }
    }
}