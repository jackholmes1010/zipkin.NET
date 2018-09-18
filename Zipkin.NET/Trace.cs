using System;

namespace Zipkin.NET
{
    public class Trace
    {
        private readonly string _traceId;
        private readonly string _spanId;
        private readonly string _parentSpanId;

        public Trace()
        {
            _traceId = GenerateTraceId();
            _spanId = GenerateTraceId();
        }

        public Trace(string traceId, string spanId)
        {
            _traceId = traceId ?? GenerateTraceId();
            _spanId = GenerateTraceId();
            _parentSpanId = spanId;
        }

        public Trace(string traceId, string spanId, string parentSpanId)
        {
            _traceId = traceId;
            _spanId = spanId;
            _parentSpanId = parentSpanId;
        }

        public SpanBuilder GetSpanBuilder(bool refresh = false)
        {
            return refresh 
                ? Refresh().GetSpanBuilder(false)
                : new SpanBuilder(_traceId, _spanId, _parentSpanId);
        }

        /// <summary>
        /// Refresh the trace ID's by setting the parent span ID
        /// to the current span ID and generating a new span ID.
        /// </summary>
        /// <returns>
        /// A new <see cref="Trace"/>.
        /// </returns>
        public Trace Refresh()
        {
            var traceId = _traceId ?? GenerateTraceId();
            return new Trace(traceId, GenerateTraceId(), _spanId);
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
