using System;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    /// <summary>
    /// Represents span context used to create child spans and propagate across process boundaries.
    /// </summary>
    public class SpanContext
    {
        /// <summary>
        /// Create a new span context.
        /// <remarks>
        /// Generates a new trace ID and span builder.
        /// </remarks>
        /// </summary>
        public SpanContext()
        {
            TraceId = GenerateTraceId();
            Id = GenerateTraceId();
        }

        /// <summary>
        /// Create a span context from a trace and span ID extracted from an upstream request.
        /// <remarks>
        /// Sets the parent span ID to the <see cref="spanId"/>
        /// and generates a new span ID for the current span context.
        /// </remarks>
        /// </summary>
        /// <param name="traceId">
        /// An existing trace ID.
        /// </param>
        /// <param name="spanId">
        /// The upstream span ID.
        /// </param>
        public SpanContext(string traceId, string spanId)
        {
            TraceId = traceId ?? GenerateTraceId();
            Id = GenerateTraceId();
            ParentId = spanId;
        }

        /// <summary>
        /// Create a span context from specified trace ID's.
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
        public SpanContext(string traceId, string spanId, string parentSpanId)
        {
            TraceId = traceId;
            Id = spanId;
            ParentId = parentSpanId;
        }

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
        /// Create a new child span context which shares a trace ID.
        /// </summary>
        /// <returns>
        /// A new <see cref="SpanContext"/>.
        /// </returns>
        public SpanContext CreateChild()
        {
            var traceId = TraceId ?? GenerateTraceId();
            return new SpanContext(traceId, GenerateTraceId(), Id)
            {
                Sampled = Sampled,
                Debug = Debug
            };
        }

        /// <summary>
        /// Use the <see cref="ISampler"/> to make a sampling decision.
        /// </summary>
        /// <returns>
        /// The current <see cref="SpanContext"/>.
        /// </returns>
        public SpanContext Sample(ISampler sampler)
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
