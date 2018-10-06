using System;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    /// <summary>
    /// Represents trace context which has come into the application with a request.
    /// </summary>
    public class TraceContext
    {
        private SpanBuilder _spanBuilder;

        /// <summary>
        /// Create a new trace context.
        /// <remarks>
        /// Generates a new trace ID and span builder.
        /// </remarks>
        /// </summary>
        public TraceContext()
        {
            TraceId = GenerateTraceId();
            Id = GenerateTraceId();
        }

        /// <summary>
        /// Create a trace context from a trace and span ID extracted from an upstream request.
        /// <remarks>
        /// Sets the parent span ID to the <see cref="spanId"/>
        /// and generates a new span ID for the current trace context.
        /// </remarks>
        /// </summary>
        /// <param name="traceId">
        /// An existing trace ID.
        /// </param>
        /// <param name="spanId">
        /// The upstream span ID.
        /// </param>
        public TraceContext(string traceId, string spanId)
        {
            TraceId = traceId ?? GenerateTraceId();
            Id = GenerateTraceId();
            ParentId = spanId;
        }

        /// <summary>
        /// Create a trace context from specified trace ID's.
        /// </summary>
        /// <param name="traceId">
        /// An existing trace ID.
        /// </param>
        /// <param name="spanId">
        /// The span ID.
        /// </param>
        /// <param name="parentSpanId">
        /// The parent span ID.
        /// </param>
        public TraceContext(string traceId, string spanId, string parentSpanId)
        {
            TraceId = traceId;
            Id = spanId;
            ParentId = parentSpanId;
        }

        /// <summary>
        /// Gets the span builder used to build spans.
        /// </summary>
        public SpanBuilder SpanBuilder => _spanBuilder ?? (_spanBuilder = new SpanBuilder(TraceId, Id, ParentId));

        /// <summary>
        /// The overall trace ID of the current trace.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// The ID of the current span.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The parent span ID.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Has the debug flag been set?
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Should this trace be sampled?
        /// <remarks>
        /// The Sample() method should be used to make a sampling decision.
        /// </remarks>
        /// </summary>
        public bool? Sampled { get; set; }

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
            return new TraceContext(traceId, GenerateTraceId(), Id)
            {
                Sampled = Sampled,
                Debug = Debug
            };
        }

        /// <summary>
        /// Use the <see cref="ISampler"/> to make a sampling decision.
        /// </summary>
        /// <returns>
        /// The current <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext Sample(ISampler sampler)
        {
            Sampled = sampler.IsSampled(this);
            return this;
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
