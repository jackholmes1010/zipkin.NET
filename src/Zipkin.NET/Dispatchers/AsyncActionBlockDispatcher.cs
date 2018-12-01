using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Zipkin.NET.Exceptions;
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
        private readonly ActionBlock<Span> _processor;

        /// <summary>
        /// Constructs a new <see cref="AsyncActionBlockDispatcher"/>.
        /// </summary>
        /// <param name="reporters">
        /// A list of reporters to which to report spans.
        /// </param>
        public AsyncActionBlockDispatcher(IEnumerable<IReporter> reporters) 
            : base (reporters)
        {
            _processor = new ActionBlock<Span>(
                async span => await SendToReporters(span), 
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

        private async Task SendToReporters(Span span)
        {
            foreach (var reporter in Reporters)
            {
                try
                {
                    await reporter.ReportAsync(span);
                }
                catch (Exception ex)
                {
                    throw new DispatchException("An error occurred reporting span.", ex);
                }
            }
        }
    }
}
