using System;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    public class TraceContext
    {
        private readonly string _spanId;
        private readonly string _parentSpanId;

        public TraceContext()
        {
            TraceId = GenerateTraceId();
            _spanId = GenerateTraceId();
        }

        public TraceContext(string traceId, string spanId)
        {
            TraceId = traceId ?? GenerateTraceId();
            _spanId = GenerateTraceId();
            _parentSpanId = spanId;
        }

        public TraceContext(string traceId, string spanId, string parentSpanId)
        {
            TraceId = traceId;
            _spanId = spanId;
            _parentSpanId = parentSpanId;
        }

        /// <summary>
        /// The overall trace ID of the current trace.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// Has the debug flag been set?
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Should this trace be sampled.
        /// <remarks>
        /// The Sample() method should be used to make a sampling decision.
        /// </remarks>
        /// </summary>
        public bool? Sampled { get; set; }

        public SpanBuilder GetSpanBuilder(bool refresh = false)
        {
            return refresh 
                ? Refresh().GetSpanBuilder(false)
                : new SpanBuilder(TraceId, _spanId, _parentSpanId);
        }

        /// <summary>
        /// Make a sampling decision.
        /// <remarks>
        /// The sampling decision is based on the presence of the sampling and debug
        /// flags. If no sampling flag exists and the debug flag has not been set,
        /// the <see cref="ISampler"/> is used to make a sampling decision.
        /// </remarks>
        /// </summary>
        /// <param name="sampler">
        /// An <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        /// <returns>
        /// The current <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext Sample(ISampler sampler)
        {
            Sampled =  Debug || sampler.IsSampled(this);
            return this;
        }

        /// <summary>
        /// Refresh the trace ID's by setting the parent span ID
        /// to the current span ID and generating a new span ID.
        /// </summary>
        /// <returns>
        /// A new <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext Refresh()
        {
            var traceId = TraceId ?? GenerateTraceId();
            return new TraceContext(traceId, GenerateTraceId(), _spanId);
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
