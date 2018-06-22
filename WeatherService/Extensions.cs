using System;

namespace WeatherService
{
    public static class Extensions
    {
        public static bool EqualsICase(this string self, string str) => self.Equals(str, StringComparison.OrdinalIgnoreCase);
    }
}