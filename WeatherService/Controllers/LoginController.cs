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
            if(User.Identity.IsAuthenticated)
            {
                return Redirect("/Home");
            }

            var m = new Login();

            if(Request.Query.ContainsKey("ReturnUrl"))
            {
                m.ReturnUrl = Request.Query["ReturnUrl"];
            }

            return View(m);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(Login m)
        {
            var result = _signInManager.PasswordSignInAsync(m.Username, m.Password, true, false).Result;

            if(result.Succeeded)
            {
                if (!string.IsNullOrEmpty(m.ReturnUrl) && Url.IsLocalUrl(m.ReturnUrl))
                {
                    return Redirect(m.ReturnUrl);
                }

                return Redirect("/Home");
            }

            m.FailureMessage = "Login failed, please check your credentials.";

            return View(m);
        }
    }
}