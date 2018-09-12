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
        private readonly ISampler _sampler;

        private bool? _sampled;

        /// <summary>
        /// Initialize with a custom trace ID generator.
        /// </summary>
        /// <param name="sampler">
        /// An <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        public TraceContext(ISampler sampler)
        {
            _sampler = sampler;
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
        /// The debug value associated with the current trace.
        /// </summary>
        /// <remarks>
        /// If true, spans will always be reported by
        /// instrumentation, regardless of the sampled value.
        /// </remarks>
        public bool? Debug { get; set; }

        /// <summary>
        /// The sampled value associated with the current trace.
        /// This SHOULD NOT be set unless a sampling decision has
        /// already been made by an upstream service.
        /// </summary>
        /// <remarks>
        /// If this is not set explicitly, a sampling
        /// decision will be made by the <see cref="ISampler"/>.
        /// </remarks>
        public bool? Sampled
        {
            get => _sampled ?? (_sampled = _sampler.IsSampled(TraceId));
            set => _sampled = value;
        }

        /// <summary>
        /// Checks if the current trace should be sampled.
        /// </summary>
        /// <remarks>
        /// If no sampling information exists a sample decision will be made.
        /// </remarks>
        /// <returns>
        /// True if the trace should be sampled.
        /// </returns>
        public bool Sample()
        {
            return Sampled == true || Debug == true;
        }

        /// <summary>
        /// Refresh the trace ID's when starting a new child trace.
        /// </summary>
        /// <returns>
        /// The same trace with a new span ID and the
        /// parent span ID is equal to the previous span ID.
        /// </returns>
        public TraceContext NewChild()
        {
            TraceId = TraceId ?? GenerateTraceId();
            ParentSpanId = SpanId ?? GenerateTraceId();
            SpanId = GenerateTraceId();
            return this;
        }

        /// <summary>
        /// Generate a 64-bit trace ID.
        /// </summary>
        /// <returns>
        /// The trace ID as a string.
        /// </returns>
        public virtual string GenerateTraceId()
        {
            // TODO this is stupid
            var random = new Random();
            var builder = new StringBuilder();
            for (var i = 0; i < 16; i++)
            {
                builder.Append(random.Next(0, 15).ToString("X").ToLower());
            }

            return builder.ToString();

            //      var bytes = new byte[8];
            //      var cryptoProvider = new RNGCryptoServiceProvider();
            //cryptoProvider.GetBytes(bytes);
            //      var id = BitConverter.ToString(bytes);
            //      return id.Replace("-", string.Empty);
        }
    }
}
