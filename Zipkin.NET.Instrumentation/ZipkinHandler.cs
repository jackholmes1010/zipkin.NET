using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> responsible for propagating
    /// X-B3 trace ID headers to downstream services and reporting
    /// completed client spans to a Zipkin server.
    /// </summary>
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

        /// <summary>
        /// Creates a new span before sending the request on to the inner
        /// handler. Records the duration and reports the completed span.
        /// </summary>
        /// <remarks>
        /// Completed spans contain both the client send and client receive times.
        /// </remarks>
        /// <param name="request">
        /// The HTTP request message to send to the server.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token to cancel operation.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
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
                TimeStamp = startTime,
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

            // Record the client receive time (span duration)
	        span.Duration = DateTime.Now.Subtract(startTime);

            // Report the complete span
            _reporter.Report(span);

            return result;
        }
	}
}
