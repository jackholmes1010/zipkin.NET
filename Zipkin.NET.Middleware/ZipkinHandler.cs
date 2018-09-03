using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Middleware
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
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IB3Propagator _propagator;

        public ZipkinHandler(
            string applicationName,
            IReporter reporter, 
            ITraceContextAccessor traceContextAccessor, 
            IB3Propagator propagator)
        {
            _applicationName = applicationName;
            _reporter = reporter;
            _traceContextAccessor = traceContextAccessor;
            _propagator = propagator;
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
            var traceContext = _traceContextAccessor.Context.StartNew();

	        // Add X-B3 headers to the outgoing request
	        request = _propagator.Inject(request, traceContext);

			var clientTrace = new ClientTrace(
		        traceContext, 
		        request.Method.ToString(), 
		        remoteEndpoint: new Endpoint
		        {
			        ServiceName = _applicationName
		        });

			// Record client send time and start duration timer
	        clientTrace.RecordStart();

	        try
	        {
		        return await base.SendAsync(request, cancellationToken);
	        }
	        catch (Exception ex)
	        {
		        clientTrace.RecordError(ex.Message);
		        throw;
	        }
	        finally
	        {
		        clientTrace.RecordEnd();

		        // Report completed span to Zipkin
				_reporter.Report(clientTrace.Span);
	        }
		}
    }
}
