using System;

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
    }
}