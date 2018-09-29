using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Zipkin.NET.Logging;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Dispatchers
{
    /// <summary>
    /// Asynchronously reports spans to a list of a 
    /// <see cref="IReporter"/>s using an <see cref="ActionBlock{TInput}"/>.
    /// </summary>
    public class AsyncActionBlockDispatcher : IDispatcher
    {
        private readonly List<IReporter> _reporters;
        private readonly IInstrumentationLogger _logger;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly ActionBlock<Span> _processor;

        /// <summary>
        /// Constructs a new <see cref="AsyncActionBlockDispatcher"/>.
        /// </summary>
        /// <param name="reporters">
        /// A list of reporters to which to report spans.
        /// </param>
        /// <param name="logger">
        /// A <see cref="IInstrumentationLogger"/> used to log instrumentation errors.
        /// </param>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor" /> used to get the current trace context.
        /// </param>
        public AsyncActionBlockDispatcher(
            IEnumerable<IReporter> reporters,
            IInstrumentationLogger logger,
            ITraceContextAccessor traceContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traceContextAccessor = traceContextAccessor ?? throw new ArgumentNullException(nameof(traceContextAccessor));
            _processor = new ActionBlock<Span>(async span => await ReportSpan(span));
            _reporters = new List<IReporter>();

            foreach (var reporter in reporters)
            {
                // TODO improve logic for preventing duplicate reporters
                var exists = _reporters.Any(r => r.GetType() == reporter.GetType());
                if (!exists)
                {
                    _reporters.Add(reporter);
                }
            }
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
                _processor.Post(span);
            }
        }

        private async Task ReportSpan(Span span)
        {
            foreach (var reporter in _reporters)
            {
                try
                {
                    await reporter.ReportAsync(span);
                }
                catch (Exception ex)
                {
                    _logger.WriteError(ex.ToString());
                }
            }
        }
    }
}
