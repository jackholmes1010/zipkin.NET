using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Zipkin.NET.Models;
using Zipkin.NET.Senders;

namespace Zipkin.NET.Reporters
{
    /// <summary>
    /// Receives and forwards spans recorded by instrumentation 
    /// to an <see cref="ISender"/> to be sent to a Zipkin server.
    /// </summary>
    public class ZipkinReporter : IReporter
    {
        private readonly ISender _sender;

        public ZipkinReporter(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Schedules a span to be sent to a transport.
        /// </summary>
        /// <remarks>
        /// The send is performed asynchronously using a <see cref="ActionBlock{TInput}"/>.
        /// </remarks>
        /// <param name="span">
        /// The trace to be reported.
        /// </param>
        public async Task ReportAsync(Span span)
        {
            await _sender.SendSpansAsync(new List<Span> { span });
        }
    }
}
