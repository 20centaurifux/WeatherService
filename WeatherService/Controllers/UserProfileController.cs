using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.View;

namespace WeatherService.Controllers
{
    public class UserProfileController : Controller
    {
        UserManager<User> _userManager;

        public UserProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            using (var db = new WeatherDb())
            {
                var user = db.User.First(u => u.UserName.ToLower().Equals(User.Identity.Name.ToLower()));

                return View(Models.User.ToViewModel(user));
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Index(UserProfile m)
        {
            if (TryValidateModel(m))
            {
                using (var db = new WeatherDb())
                {
                    if (!string.IsNullOrEmpty(m.Email) && db.User.Any(u => u.Email != null && u.Email.ToLower().Equals(m.Email.ToLower()) && !u.Id.Equals(m.Id)))
                    {
                        ViewData["ValidationError"] = "The email address is already assigned.";
                    }
                    else
                    {
                        var user = db.User.First(u => u.Id.Equals(m.Id));
                        var result = IdentityResult.Success;

                        if (!string.IsNullOrEmpty(m.Password))
                        {
                            if (m.Password.Equals(m.Password2))
                            {
                                result = _userManager.RemovePasswordAsync(user).Result;

                                if (result.Succeeded)
                                {
                                    result = _userManager.AddPasswordAsync(user, m.Password).Result;
                                }
                            }
                            else
                            {
                                result = IdentityResult.Failed(new IdentityError[] { new IdentityError() { Description = "Entered passwords must be equal." } });
                            }
                        }

                        if (result.Succeeded)
                        {
                            user.Email = m.Email;

                            result = _userManager.UpdateAsync(user).Result;
                        }

                        if (!result.Succeeded)
                        {
                            if (result.Errors.Count() > 0)
                            {
                                ViewData["ValidationError"] = result.Errors.First().Description;
                            }
                            else
                            {
                                ViewData["ValidationError"] = "Update failed, please check entered data and try again.";
                            }
                        }
                    }
                }
            }
            else
            {
                ViewData["ValidationError"] = "Couldn't validate input, please check entered data.";
            }

            m.Password = string.Empty;
            m.Password2 = string.Empty;

            return View(m);
        }
    }
}