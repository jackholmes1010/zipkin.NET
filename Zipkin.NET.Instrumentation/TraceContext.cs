using System;
using System.Text;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// Class which contains context information about the the current trace.
    /// </summary>
    public class TraceContext
    {
        private readonly Random _random;

        public TraceContext()
        {
            _random = new Random();
        }

        /// <summary>
        /// The trace ID value associated with the current trace.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// The span ID value associated with the current trace.
        /// </summary>
        public string SpanId { get; set; }

        /// <summary>
        /// The parent span ID associated with the current trace.
        /// </summary>
        public string ParentSpanId { get; set; }

        /// <summary>
        /// The sampled value associated with the current trace.
        /// </summary>
        public bool? Sampled { get; set; }

        /// <summary>
        /// The debug values associated with the current trace.
        /// </summary>
        public bool? Debug { get; set; }

        /// <summary>
        /// Refresh the trace ID's when starting a new child trace.
        /// </summary>
        /// <returns>
        /// A new <see cref="TraceContext"/> instance with a new
        /// span ID and parent span ID equal to the previous span ID.
        /// </returns>
        public TraceContext NewChildTrace()
        {
            return new TraceContext
            {
                TraceId = TraceId ?? GenerateTraceId(_random),
                ParentSpanId = SpanId ?? GenerateTraceId(_random),
                SpanId = GenerateTraceId(_random),
                Debug = Debug,
                Sampled = Sampled
            };
        }

        /// <summary>
        /// Make a sampling decision based on the value of the parent's
        /// sampling decision and the debug flag. If the debug flag is 
        /// not set and no sampling decision has been made by an upstream 
        /// service, make a sampling decisision using the <see cref="ISampler"/>.
        /// </summary>
        /// <param name="sampler">
        /// An <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        /// <returns>
        /// The current <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext Sample(ISampler sampler)
        {
            if (Debug == true)
                Sampled = true;

            if (!Sampled.HasValue)
                Sampled = sampler.IsSampled(this);

            return this;
        }

        /// <summary>
        /// Generate a 64-bit trace ID.
        /// </summary>
        /// <returns>
        /// The trace ID as a string.
        /// </returns>
        public virtual string GenerateTraceId(Random random = null)
        {
			return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
		}
    }
}
