using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Instrumentation
{
    public class ZipkinHandler : DelegatingHandler
    {
	    private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ITraceContext _traceContext;
        private readonly ITraceIdentifierGenerator _traceIdGenerator;

        public ZipkinHandler(
			string applicationName,
            IReporter reporter, 
            ITraceContext traceContext,
            ITraceIdentifierGenerator traceIdGenerator)
        {
	        _applicationName = applicationName;
            _reporter = reporter;
            _traceContext = traceContext;
            _traceIdGenerator = traceIdGenerator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Record the client send time (span timestamp)
	        var startTime = DateTime.Now;

            var span = new Span
            {
                Id = _traceIdGenerator.GenerateId(),
                TraceId = _traceContext.CurrentTraceId,
                ParentId = _traceContext.CurrentSpanId,
                Timestamp = startTime,
				Name = request.Method.ToString(),
                Kind = SpanKind.Client,
				RemoteEndpoint = new Endpoint
				{
					ServiceName = _applicationName
				}
			};

            // Add X-B3 headers to the outgoing request
            request.Headers.Add("X-B3-TraceId", span.TraceId);
            request.Headers.Add("X-B3-SpanId", span.Id);
            request.Headers.Add("X-B3-ParentSpanId", span.ParentId);

            var result = await base.SendAsync(request, cancellationToken);

            // Record the client recieve time (span duration)
	        span.Duration = DateTime.Now.Subtract(startTime);

            // Report the complete span
            await _reporter.ReportAsync(span);

            return result;
        }
	}
}
