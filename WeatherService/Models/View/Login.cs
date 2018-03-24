namespace WeatherService.Models.View
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public string FailureMessage { get; set; }
    }
}