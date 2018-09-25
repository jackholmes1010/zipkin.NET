using System;
using System.Collections.Generic;
using Zipkin.NET.Models;

namespace Zipkin.NET
{
    public class SpanBuilder
    {
        private readonly Span _span;

        public SpanBuilder(string traceId, string id, string parentId)
        {
            _span = new Span
            {
                TraceId = traceId,
                Id = id,
                ParentId = parentId
            };
        }

        /// <summary>
        /// Record the start time and start duration timer.
        /// </summary>
        public SpanBuilder Start()
        {
            _span.StartTime = DateTime.UtcNow;
            return this;
        }

        /// <summary>
        /// Calculate the duration from the time the  start time was recorded.
        /// </summary>
        public SpanBuilder End()
        {
            _span.Duration = DateTime.UtcNow.Subtract(_span.StartTime);
            return this;
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
        public SpanBuilder Tag(string name, string value)
        {
            if (_span.Tags == null)
                _span.Tags = new Dictionary<string, string>();

            _span.Tags.Add(name, value);
            return this;
        }

        /// <summary>
        /// Add error context information to a trace.
        /// </summary>
        /// <param name="message">
        /// The error message.
        /// </param>
        public SpanBuilder Error(string message)
        {
            Tag("error", message);
            return this;
        }

        /// <summary>
        /// Add an event that explains latency with a timestamp.
        /// </summary>
        /// <param name="annotation">
        /// The value and associated timestamp.
        /// </param>
        public SpanBuilder Annotate(Annotation annotation)
        {
            if (_span.Annotations == null)
                _span.Annotations = new List<Annotation>();

            _span.Annotations.Add(annotation);
            return this;
        }

        public SpanBuilder WithLocalEndpoint(Endpoint endpoint)
        {
            _span.LocalEndpoint = endpoint;
            return this;
        }

        public SpanBuilder WithRemoteEndpoint(Endpoint endpoint)
        {
            _span.RemoteEndpoint = endpoint;
            return this;
        }

        public SpanBuilder Kind(SpanKind kind)
        {
            _span.Kind = kind;
            return this;
        }

        /// <summary>
        /// Build the span.
        /// </summary>
        /// <returns>
        /// A <see cref="Span"/>.
        /// </returns>
        public Span Build()
        {
            return _span;
        }
    }
}
