using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using WeatherService.Models;

namespace WeatherService.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(int statusCode)
        {
            var m = new HttpError() { StatusCode = 200, Message = "I don't know what's going on." };

            try
            {
                var msg = new HttpResponseMessage((HttpStatusCode)statusCode);

                m.StatusCode = statusCode;
                m.Message = msg.ReasonPhrase;
            }
            catch
            {
                if (statusCode == 1337)
                {
                    m.Message = "ph342 m9 1337 h4xX0r 5k!11Zz!!";
                }
                else if (statusCode == 23)
                {
                    m.Message = "If you can't see the FNORD it can't eat you.";
                }
                else if (statusCode == 42)
                {
                    m.Message = "Goodbye and thank you for the fish.";
                }
            }

            return View("Error", m);
        }

        public IActionResult AccessDenied()
        {
            return Redirect("/Error?statusCode=403");
        }
    }
}