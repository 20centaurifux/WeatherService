using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherService.Models.View;

namespace WeatherService.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(int statusCode)
        {
            var m = new HttpError() { StatusCode = 200, Message = "I don't know what's going on." };

            m.StatusCode = statusCode;

            if (statusCode == 0x539)
            {
                m.Message = "ph342 m9 1337 h4xX0r 5k!11Zz!!";
            }
            else if (statusCode == 0x17)
            {
                m.Message = "If you can't see the FNORD it can't eat you.";
            }
            else if (statusCode == 0x2A)
            {
                m.Message = "Goodbye and thank you for the fish.";
            }
            else
            {
                try
                {
                    var msg = new HttpResponseMessage((HttpStatusCode)statusCode);

                    m.Message = msg.ReasonPhrase;

                    if (!string.IsNullOrEmpty(msg.ReasonPhrase))
                    {
                        m.Message = msg.ReasonPhrase;
                    }
                }
                catch { }
            }

            return View("Error", m);
        }

        public IActionResult AccessDenied()
        {
            return Redirect("/Error?statusCode=403");
        }
    }
}