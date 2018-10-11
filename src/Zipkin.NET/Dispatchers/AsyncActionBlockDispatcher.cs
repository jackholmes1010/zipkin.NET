using System;
using System.Collections.Generic;
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
    public class AsyncActionBlockDispatcher : Dispatcher, IDisposable
    {
        private readonly IInstrumentationLogger _logger;
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
        public AsyncActionBlockDispatcher(
            IEnumerable<IReporter> reporters,
            IInstrumentationLogger logger) 
            : base (reporters)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processor = new ActionBlock<Span>(
                async span => await ReportSpan(span), 
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 10
                });
        }

        /// <summary>
        /// True if there are no remaining spans to be processed.
        /// </summary>
        /// <returns></returns>
        public bool IsCompleted()
        {
            return _processor.InputCount == 0;
        }

        /// <summary>
        /// Complete span processing.
        /// </summary>
        public void Dispose()
        {
            _processor.Complete();
        }

        /// <summary>
        /// Schedule a completed <see cref="Span"/> to be reported to all of the available reporters.
        /// </summary>
        /// <param name="span">
        /// A complete span.
        /// </param>
        protected  override void Schedule(Span span)
        {
            _processor.Post(span);
        }

        private async Task ReportSpan(Span span)
        {
            foreach (var reporter in Reporters)
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
