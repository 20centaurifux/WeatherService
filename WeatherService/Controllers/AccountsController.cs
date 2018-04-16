using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using WeatherService.Data;
using WeatherService.Models;
using WeatherService.Models.View;
using LinqToDB;

namespace WeatherService.Controllers
{
    public class AccountsController : Controller
    {
        UserManager<User> _userManager;

        public AccountsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            using (var db = new WeatherDb())
            {
                return View(db.User.Select(u => ToUserProfile(u)).ToArray());
            }
        }

        [HttpDelete("Accounts/{id?}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if(user.UserName.ToLower().Equals(User.Identity.Name.ToLower()))
            {
                return Forbid();
            }

            if (user.UserName.Equals("Admin"))
            {
                return Forbid();
            }

            var result = _userManager.DeleteAsync(user).Result;

            if (result.Succeeded)
            {
                using (var db = new WeatherDb())
                {
                    if (db.DataProvider.Name.Equals("SQLite"))
                    {
                        db.BeginTransaction();

                        try
                        {
                            DashboardHelper.DeleteDashboardItems(db, user.Id);

                            db.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            db.RollbackTransaction();
                            throw ex;
                        }
                    }
                }

                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("Accounts/Edit/{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(string id)
        {
            using (var db = new WeatherDb())
            {
                var u = db.User.FirstOrDefault(s => s.Id.Equals(id));

                if (u == null)
                {
                    return NotFound();
                }

                return View("EditAccount", ToUserProfile(u));
            }
        }

        [HttpPost("Accounts/Update")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(UserProfile m)
        {
            if (TryValidateModel(m))
            {
                using (var db = new WeatherDb())
                {
                    if (db.User.Any(u => u.UserName.ToLower().Equals(m.Username.ToLower()) && !u.Id.Equals(m.Id)))
                    {
                        ViewData["ValidationError"] = "A user with the given name does already exist.";
                    }
                    else if (!string.IsNullOrEmpty(m.Email) && db.User.Any(u => u.Email != null && u.Email.ToLower().Equals(m.Email.ToLower()) && !u.Id.Equals(m.Id)))
                    {
                        ViewData["ValidationError"] = "The email address is already assigned.";
                    }
                    else
                    {
                        var user = db.User.First(u => u.Id.Equals(m.Id));
                        var result = IdentityResult.Success;

                        if (!string.IsNullOrEmpty(m.Password))
                        {
                            result = _userManager.RemovePasswordAsync(user).Result;

                            if (result.Succeeded)
                            {
                                result = _userManager.AddPasswordAsync(user, m.Password).Result;
                            }
                        }

                        if (result.Succeeded)
                        {
                            user.Email = m.Email;

                            if (!user.UserName.Equals("Admin"))
                            {
                                user.UserName = m.Username;

                                result = _userManager.UpdateAsync(user).Result;

                                if (result.Succeeded)
                                {
                                    if (m.IsAdmin && !_userManager.IsInRoleAsync(user, "Administrator").Result)
                                    {
                                        result = _userManager.RemoveFromRoleAsync(user, "Reader").Result;

                                        if (result.Succeeded)
                                        {
                                            result = _userManager.AddToRoleAsync(user, "Administrator").Result;
                                        }
                                    }
                                    else if (!m.IsAdmin && _userManager.IsInRoleAsync(user, "Administrator").Result)
                                    {
                                        result = _userManager.RemoveFromRoleAsync(user, "Administrator").Result;

                                        if (result.Succeeded)
                                        {
                                            result = _userManager.AddToRoleAsync(user, "Reader").Result;
                                        }
                                    }
                                }
                            }
                        }

                        if (result.Succeeded)
                        {
                            return Redirect("/Accounts");
                        }
                        else if (result.Errors.Count() > 0)
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
            else
            {
                ViewData["ValidationError"] = "Couldn't validate input, please check entered data.";
            }

            return View("EditAccount", m);
        }

        [HttpGet("Accounts/Create")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View("CreateAccount", new UserProfile() { Username = "JohnDoe", IsAdmin = false });
        }

        [HttpPost("Accounts/Create")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(UserProfile m)
        {
            if (TryValidateModel(m))
            {
                using (var db = new WeatherDb())
                {
                    if (db.User.Any(u => u.UserName.ToLower().Equals(m.Username.ToLower())))
                    {
                        ViewData["ValidationError"] = "A user with the given name does already exist.";
                    }
                    else if (!string.IsNullOrEmpty(m.Email) && (db.User.Any(u => u.Email != null && u.Email.ToLower().Equals(m.Email.ToLower()))))
                    {
                        ViewData["ValidationError"] = "The email address is already assigned.";
                    }
                    else
                    {
                        var user = new User
                        {
                            Id = m.Id,
                            UserName = m.Username,
                            Email = m.Email
                        };

                        if (m.Password == null)
                        {
                            m.Password = string.Empty;
                        }

                        var result = _userManager.CreateAsync(user, m.Password).Result;

                        if(result.Succeeded)
                        {
                            if(m.IsAdmin)
                            {
                                result = _userManager.AddToRoleAsync(user, "Administrator").Result;
                            }
                            else
                            {
                                result = _userManager.AddToRoleAsync(user, "Reader").Result;
                            }
                        }

                        if(result.Succeeded)
                        {
                            return Redirect("/Accounts");
                        }
                        else if (result.Errors.Count() > 0)
                        {
                            ViewData["ValidationError"] = result.Errors.First().Description;
                        }
                        else
                        {
                            ViewData["ValidationError"] = "Updated failed, please check entered data and try again.";
                        }
                    }
                }
            }
            else
            {
                ViewData["ValidationError"] = "Couldn't validate input, please check entered data.";
            }

            return View("CreateAccount", m);
        }

        private UserProfile ToUserProfile(User user)
        {
            var p = Models.User.ToViewModel(user);

            p.IsAdmin = _userManager.IsInRoleAsync(user, "Administrator").Result;

            return p;
        }
    }
}