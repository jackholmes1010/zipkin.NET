namespace Zipkin.NET.Constants
{
    /// <summary>
    /// X-B3 trace propagation header constants.
    /// </summary>
    public class B3HeaderConstants
    {
        /// <summary>
        /// The trace ID header name.
        /// </summary>
        public static string TraceId = "X-B3-TraceId";

        /// <summary>
        /// The span ID header name.
        /// </summary>
        public static string SpanId = "X-B3-SpanId";

        /// <summary>
        /// The parent span ID header name.
        /// </summary>
        public static string ParentSpanId = "X-B3-ParentSpanId";

        /// <summary>
        /// The sampled header name.
        /// </summary>
        public static string Sampled = "X-B3-Sampled";

        /// <summary>
        /// The flags (debug) header name.
        /// </summary>
        public static string Flags = "X-B3-Flags";
    }
}
