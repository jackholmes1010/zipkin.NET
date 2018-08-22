using System.Collections.Generic;
using System.Threading.Tasks;
using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation.Reporting
{
    public class Reporter : IReporter
    {
        private readonly ISender _sender;

        public Reporter(ISender sender)
        {
            _sender = sender;
        }

        public async Task ReportAsync(Span span)
        {
	        await _sender.SendSpansAsync(new List<Span> {span});
        }
    }
}
