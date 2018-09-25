using System;

namespace Zipkin.NET.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static TimeSpan ToUnixTime(this DateTime dateTime)
        {
            var unixTime = dateTime.ToUniversalTime().Subtract(Epoch);
            return unixTime;
        }
    }
}
