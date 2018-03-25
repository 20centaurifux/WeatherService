using System;
using System.Globalization;

namespace WeatherService.Utils
{
    public abstract class DateTimeConverter
    {
        public static DateTime UnixTimestampToDateTime(long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);

            return dtDateTime;
        }

        public static bool ParseDateTimeString(string str, ref DateTime dt)
        {
            var success = false;
            var provider = CultureInfo.InvariantCulture;

            try
            {
                dt = DateTime.ParseExact(str, "yyyy-MM-dd", provider);
                success = true;
            }
            catch { }

            return success;
        }

        public static DateTime BeginningOfDay(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
        }

        public static DateTime EndOfDay(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
        }
    }
}