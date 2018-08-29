using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation.Reporting
{
    public class Reporter : IReporter, IDisposable
    {
        private readonly ISender _sender;
        private readonly ActionBlock<Span> _spanProcessor;

        public Reporter(ISender sender)
        {
            _sender = sender;
            _spanProcessor = new ActionBlock<Span>(async span => await SendSpan(span));
        }

        public void Report(Span span)
        {
            _spanProcessor.Post(span);
        }

        private async Task SendSpan(Span span)
        {
            try
            {
                await _sender.SendSpansAsync(new List<Span> {span});
            }
            catch (Exception ex)
            {
                // TODO log exception maybe?
            }
        }

        public void Dispose()
        {
            // TODO is this necessary?
            _spanProcessor.Complete();
        }
    }
}
