using System;

namespace Zipkin.NET.Extensions
{
    public static class TimeSpanExtensions
    {
        private const int TicksPerMicrosecond = 10;

        public static long ToMicroseconds(this TimeSpan timeSpan)
        {
            return (int)Math.Floor(
                timeSpan.Ticks % TimeSpan.TicksPerMillisecond 
                / (double)TicksPerMicrosecond);
        }
    }
}
