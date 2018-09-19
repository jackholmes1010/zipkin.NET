using System;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    public class Trace
    {
        private readonly string _spanId;
        private readonly string _parentSpanId;

        public Trace()
        {
            TraceId = GenerateTraceId();
            _spanId = GenerateTraceId();
        }

        public Trace(string traceId, string spanId)
        {
            TraceId = traceId ?? GenerateTraceId();
            _spanId = GenerateTraceId();
            _parentSpanId = spanId;
        }

        public Trace(string traceId, string spanId, string parentSpanId)
        {
            TraceId = traceId;
            _spanId = spanId;
            _parentSpanId = parentSpanId;
        }

        public string TraceId { get; set; }

        public bool Debug { get; set; }

        public bool? Sampled { get; set; }

        public SpanBuilder GetSpanBuilder(bool refresh = false)
        {
            return refresh 
                ? Refresh().GetSpanBuilder(false)
                : new SpanBuilder(TraceId, _spanId, _parentSpanId);
        }

        public Trace Sample(ISampler sampler)
        {
            Sampled =  Debug || sampler.IsSampled(this);
            return this;
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
            var traceId = TraceId ?? GenerateTraceId();
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
