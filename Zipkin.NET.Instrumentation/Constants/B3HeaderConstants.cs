namespace Zipkin.NET.Instrumentation.Constants
{
    public class B3HeaderConstants
    {
        public static string TraceId = "X-B3-TraceId";
        public static string SpanId = "X-B3-SpanId";
        public static string ParentSpanId = "X-B3-ParentSpanId";
        public static string Sampled = "X-B3-Sampled";
        public static string Flags = "X-B3-Flags";
    }
}
