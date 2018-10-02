using System;

namespace Zipkin.NET.Extensions
{
    public static class TimeSpanExtensions
    {
        public static long ToMicroseconds(this TimeSpan timeSpan)
        {
            var milliseconds = timeSpan.Ticks / 10000.0;
            var microseconds = milliseconds * 1000.0;
            return (long) microseconds;
        }
    }
}
