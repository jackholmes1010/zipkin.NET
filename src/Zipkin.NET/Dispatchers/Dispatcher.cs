using System;
using System.Collections.Generic;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Dispatchers
{
    /// <summary>
    /// Dispatch a completed span to available reporters.
    /// </summary>
    public abstract class Dispatcher : IDispatcher
    {
        protected Dispatcher(IEnumerable<IReporter> reporters)
        {
            Reporters = reporters ?? throw new ArgumentNullException(nameof(reporters));
        }

        /// <summary>
        /// Gets a list of available reporters used to report spans.
        /// </summary>
        protected IEnumerable<IReporter> Reporters { get; }

        /// <summary>
        /// Post a span to all available reporters.
        /// <remarks>
        /// Spans will only be reported if the current SpanContext Sampled 
        /// property is not null, i.e. a sampling decision has been made.
        /// </remarks>
        /// </summary>
        /// <param name="span">
        /// A complete span.
        /// </param>
        public void Dispatch(Span span)
        {
            if (span.SpanContext.Sampled == null)
            {
                throw new Exception(
                    "TraceContext.Sampled property has not been set. Call Tracer.Sample() to set the Sampled property before reporting span.");
            }

            if (span.SpanContext.Sampled == true)
            {
                Schedule(span);
            }
        }

        /// <summary>
        /// Schedule a span to be sent to available reporters.
        /// </summary>
        /// <param name="span">
        /// The completed <see cref="Span"/>.
        /// </param>
        protected abstract void Schedule(Span span);
    }
}
