using System;
using Zipkin.NET.Models;

namespace Zipkin.NET.Dispatchers
{
    /// <summary>
    /// Dispatch a completed span to available reporters.
    /// </summary>
    public abstract class Dispatcher
    {
        private readonly ITraceContextAccessor _traceContextAccessor;

        protected Dispatcher(ITraceContextAccessor traceContextAccessor)
        {
            _traceContextAccessor = traceContextAccessor 
                ?? throw new ArgumentNullException(nameof(traceContextAccessor));
        }

        /// <summary>
        /// Schedule a completed <see cref="Span"/> to be reported to all of the available reporters.
        /// <remarks>
        /// Spans will only be reportered if the current <see cref="TraceContext"/> Sampled 
        /// property is not null, i.e. a sampling decision has been made for the trace.
        /// </remarks>
        /// </summary>
        /// <param name="span">
        /// A complete span.
        /// </param>
        public void Dispatch(Span span)
        {
            if (!_traceContextAccessor.HasTrace())
            {
                throw new Exception(
                    "Save the TraceContext using the ITraceContextContextAccessor.SaveTrace() before dispatching span.");
            }

            var traceContext = _traceContextAccessor.GetTrace();

            if (traceContext.Sampled == null)
            {
                throw new Exception(
                    "TraceContext.Sampled property has not been set. Call Tracer.Sample() to set the Sampled property before reporting span.");
            }

            if (traceContext.Sampled == true)
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
