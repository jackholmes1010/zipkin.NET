using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation.Reporting
{
    /// <summary>
    /// Receives and asyhcronously forwards spans recorded by
    /// instrumentation to an <see cref="ISender"/> to be sent to a Zipkin server.
    /// </summary>
    public class Reporter : IReporter, IDisposable
    {
        private readonly ISender _sender;
        private readonly ActionBlock<Span> _processor;

        public Reporter(ISender sender)
        {
            _sender = sender;
            _processor = new ActionBlock<Span>(async span => await SendSpan(span));
        }

        /// <summary>
        /// Schedules a span to be sent to a transport.
        /// </summary>
        /// <remarks>
        /// The send is performed asynchronously using a <see cref="ActionBlock{TInput}"/>.
        /// </remarks>
        /// <param name="span">
        /// The span to be reported.
        /// </param>
        public void Report(Span span)
        {
            _processor.Post(span);
        }

        /// <summary>
        /// Complete reporting of spans.
        /// </summary>
        public void Dispose()
        {
            _processor.Complete();
        }

        private async Task SendSpan(Span span)
        {
            try
            {
                await _sender.SendSpansAsync(new List<Span> {span});
            }
            catch (Exception)
            {
                // TODO Maybe log exception?
            }
        }
    }
}
