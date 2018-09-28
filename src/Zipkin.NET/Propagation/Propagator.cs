using System;
using Zipkin.NET.Models;

namespace Zipkin.NET.Propagation
{
    /// <summary>
    /// Propagators are used to pass trace context between applications.
    /// </summary>
    /// <typeparam name="TInject">
    /// The type on which to inject a <see cref="Span"/>.
    /// </typeparam>
    public abstract class Propagator<TInject>
    {
        public TInject Propagate(TInject inject, TraceContext traceContext)
        {
            if (traceContext.Sampled == null)
            {
                throw new Exception(
                    "TraceContext Sampled property is null. Make a sampling decision before propagating trace context using TraceContext.Sample().");
            }

            return Inject(inject, traceContext);
        }

        /// <summary>
        /// Inject <see cref="Span"/> details into a <see cref="TInject"/>.
        /// </summary>
        /// <param name="inject">
        /// The object to inject span details into.
        /// </param>
        /// <param name="traceContext">
        /// The trace context which contains trace ID, sampling and debug info.
        /// </param>
        /// <returns>
        /// The <see cref="TInject"/> object with span details.
        /// </returns>
        protected abstract TInject Inject(TInject inject, TraceContext traceContext);
    }
}
