using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Models;
using WeatherService.Models.View;

namespace WeatherService.Controllers
{
    public class LoginController : Controller
    {
        SignInManager<User> _signInManager;

        public LoginController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(Login m)
        {
            var result = _signInManager.PasswordSignInAsync(m.Username, m.Password, true, false).Result;

            if(result.Succeeded)
            {
                return Redirect("/Home");
            }

            m.FailureMessage = "Login failed, please check your credentials.";

            return View(m);
        }
    }
}