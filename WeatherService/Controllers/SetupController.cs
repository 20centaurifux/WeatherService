using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeatherService.Data;
using WeatherService.Models;
using LinqToDB;

namespace WeatherService.Controllers
{
    public class SetupController : Controller
    {
        readonly RoleManager<UserRole> _roleManager;
        readonly UserManager<User> _userManager;

        public SetupController(RoleManager<UserRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var message = "Upgrade not required.";

            using (var db = new WeatherDb())
            {
                var info = db.MetaInfo.FirstOrDefault();

                if (info == null)
                {
                    var result = CreateDefaultRoles();

                    if(result.Succeeded)
                    {
                        var gen = new PasswordGenerator(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true, passwordLength: 8);
                        var password = gen.Next();

                        result = CreateAdminUser(password);

                        if (result.Succeeded)
                        {
                            var admin = _userManager.FindByNameAsync("Admin").Result;

                            if (admin == null)
                            {
                                message = "Admin account not found.";
                            }
                            else
                            {
                                result = _userManager.AddToRoleAsync(admin, "Administrator").Result;

                                if (result.Succeeded)
                                {
                                    info = new MetaInfo() { DBRevision = 1 };
                                    db.Insert(info);

                                    message = string.Format("Upgraded to database revision 1. \"Admin\" account with password \"{0}\" created.", password);
                                }
                            }
                        }
                    }

                    if(!result.Succeeded)
                    {
                        message = ErrorMessageFromResult(result);
                    }
                }
            }

            return Content(message);
        }

        IdentityResult CreateDefaultRoles()
        {
            IdentityResult result = null;

            foreach (var roleName in new string[] { "Administrator", "Reader" })
            {
                result = _roleManager.CreateAsync(new UserRole() { Name = roleName }).Result;

                if(!result.Succeeded)
                {
                    break;
                }
            }

            return result;
        }

        IdentityResult CreateAdminUser(string password)
        {
            var adminRole = _roleManager.FindByNameAsync("Adminstrator").Result;

            return _userManager.CreateAsync(new Models.User() { UserName = "Admin" }, password).Result;
        }

        static string ErrorMessageFromResult(IdentityResult result)
        {
            var message = "Couldn't upgrade database.";

            if (result.Errors.Count() > 0)
            {
                message = string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return message;
        }
    }
}