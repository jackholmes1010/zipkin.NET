using System;

namespace Zipkin.NET
{
    public class Trace
    {
        /// <summary>
        /// The overall ID of the current trace.
        /// </summary>
        private readonly string _traceId;

        /// <summary>
        /// The ID of the previous span.
        /// <remarks>
        /// This will become the parent ID of built spans.
        /// </remarks>
        /// </summary>
        private readonly string _spanId;

        public Trace()
        {
            _traceId = GenerateTraceId();
        }

        public Trace(string traceId, string spanId)
        {
            _traceId = traceId ?? GenerateTraceId();
            _spanId = spanId;
        }

        public SpanBuilder GetSpanBuilder()
        {
            var id = GenerateTraceId();
            return new SpanBuilder(_traceId, id, _spanId);
        }

        /// <summary>
        /// Generate a 64-bit trace ID.
        /// </summary>
        /// <returns>
        /// The trace ID as a string.
        /// </returns>
        public string GenerateTraceId()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        }
    }
}
