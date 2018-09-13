using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// Base trace class acting as a convenience wrapper around a <see cref="Models.Span"/>.
    /// </summary>
    public abstract class Trace
    {
        private Stopwatch _timer;

        protected Trace(
            TraceContext traceContext, 
            string name,
            Endpoint localEndpoint = null, 
            Endpoint remoteEndpoint = null)
        {
            TraceContext = traceContext;

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

        /// <summary>
        /// Check if the current trace should be sampled.
        /// <remarks>
        /// Simple pass through to <see cref="TraceContext.Sampled"/>.
        /// </remarks>
        /// </summary>
        /// <returns>
        /// True if the trace should be sampled.
        /// </returns>
        public bool Sampled()
        {
            return TraceContext.Sampled == true;
        }
    }
}
