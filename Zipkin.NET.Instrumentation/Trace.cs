using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// Base trace class acting as a convenience wrapper around a <see cref="Models.Span"/>.
    /// </summary>
    public abstract class Trace
    {
        private readonly ISampler _sampler;

        private Stopwatch _timer;
        private bool? _sampled;

        protected Trace(
            ISampler sampler,
            TraceContext traceContext, 
            string name,
            Endpoint localEndpoint = null, 
            Endpoint remoteEndpoint = null)
        {
            _sampler = sampler;

            TraceContext = traceContext;

            // The sampled value should only be set if the debug flag
            // is true, or a sampling decision has already been made,
            // i.e. TraceContext.Sampled is not null.
            if (traceContext.Debug == true)
            {
                Sampled = true;
            }
            else
            {
                if (traceContext.Sampled != null &&
                    traceContext.Sampled == true)
                {
                    Sampled = true;
                }
            }

            Span = new Span
            {
                Name = name,
                Id = traceContext.SpanId,
                TraceId = traceContext.TraceId,
                ParentId = traceContext.ParentSpanId,
                LocalEndpoint = localEndpoint,
                RemoteEndpoint = remoteEndpoint
            };
        }

        /// <summary>
        /// The span associated with this trace.
        /// </summary>
        public Span Span { get; }

        /// <summary>
        /// Trace ID's associated with the current trace.
        /// </summary>
        public TraceContext TraceContext { get; }

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
            get => _sampled ?? (_sampled = _sampler.IsSampled(this));
            set => _sampled = value;
        }

        /// <summary>
        /// Refresh the trace ID's when starting a new child trace.
        /// </summary>
        /// <returns>
        /// The same trace with a new span ID and the
        /// parent span ID is equal to the previous span ID.
        /// </returns>
        public Trace NewChild()
        {
            TraceContext.TraceId = TraceContext.TraceId ?? GenerateTraceId();
            TraceContext.ParentSpanId = TraceContext.SpanId ?? GenerateTraceId();
            TraceContext.SpanId = GenerateTraceId();
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

        /// <summary>
        /// Record the start time and start duration timer.
        /// </summary>
        public void Start()
        {
            Span.StartTime = DateTime.Now;
            _timer = new Stopwatch();
            _timer.Start();
        }

        /// <summary>
        /// Calculate the duration from the time the  start time was recorded.
        /// </summary>
        public void End()
        {
            if (_timer == null)
                return;

            _timer.Stop();
            Span.Duration = _timer.Elapsed;
        }

        /// <summary>
        /// Add additional context information to a trace.
        /// </summary>
        /// <param name="name">
        /// The tag key. This should be unique.
        /// </param>
        /// <param name="value">
        /// The tag value.
        /// </param>
        public void Tag(string name, string value)
        {
            if (Span.Tags == null)
                Span.Tags = new Dictionary<string, string>();

            Span.Tags.Add(name, value);
        }

        /// <summary>
        /// Add error context information to a trace.
        /// </summary>
        /// <param name="message">
        /// The error message.
        /// </param>
        public void Error(string message)
        {
            Tag("error", message);
        }

        /// <summary>
        /// Add an event that explains latency with a timestamp.
        /// </summary>
        /// <param name="annotation">
        /// The value and associated timestamp.
        /// </param>
        public void Annotate(Annotation annotation)
        {
            if (Span.Annotations == null)
                Span.Annotations = new List<Annotation>();

            Span.Annotations.Add(annotation);
        }
    }
}
