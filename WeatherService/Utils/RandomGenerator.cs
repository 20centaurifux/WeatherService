using System;
using System.Linq;

namespace WeatherService.Utils
{
    public class RandomGenerator
    {
        public static readonly string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly string NUMBERS = "0123456789";

        private Random _rnd = new Random();

        public RandomGenerator()
        {
            _rnd = new Random();
        }

        public string Generate(string characters, int length)
        {
            return new String(characters.Select(c => characters[_rnd.Next(characters.Length)]).Take(length).ToArray());
        }

        public string GenerateAlphabetic()
        {
            return GenerateAlphabetic(_rnd.Next(8));
        }

        public string GenerateAlphabetic(int length)
        {
            return Generate(CHARS, length);
        }

        public string GenerateAlphanumeric(int length)
        {
            return Generate(CHARS + NUMBERS, length);
        }

        public string GenerateAlphanumeric()
        {
            return GenerateAlphanumeric(_rnd.Next(8));
        }

        public string AppendParameterToUrl(string url)
        {
            if (url.EndsWith("/") || url.EndsWith("&") || url.EndsWith("?"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            var paramName = GenerateAlphabetic();
            var found = true;

            for (var i = 0; found && i < 1337; ++i)
            {
                if(url.Contains(paramName + "="))
                {
                    paramName = GenerateAlphabetic();
                }
                else
                {
                    found = false;
                }
            }

            if (!found)
            {
                var concat = '?';

                if (url.Contains("?"))
                {
                    concat = '&';
                }

                url = string.Format("{0}{1}{2}={3}", url, concat, paramName, GenerateAlphanumeric(32));
            }

            return url;
        }
    }
}