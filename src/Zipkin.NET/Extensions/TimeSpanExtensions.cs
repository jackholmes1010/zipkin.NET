using System;

namespace Zipkin.NET.Extensions
{
    public static class TimeSpanExtensions
    {
        public static long ToMicroseconds(this TimeSpan timeSpan)
        {
            var milliseconds = timeSpan.Ticks / TimeSpan.TicksPerMillisecond;
            var microseconds = milliseconds * 1000;
            return microseconds;
        }
    }
}
