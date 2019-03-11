using System;
using System.Text;

namespace WeatherService
{
    public static class Extensions
    {
        public static bool EqualsICase(this string self, string str) => self.Equals(str); // self.Equals(str, StringComparison.OrdinalIgnoreCase);

        public static string ToHexString(this byte[] self)
        {
            StringBuilder hex = new StringBuilder(self.Length * 2);

            foreach (var b in self)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}