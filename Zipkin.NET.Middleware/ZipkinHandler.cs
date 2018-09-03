using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;
using Trace = Zipkin.NET.Instrumentation.Trace;

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
            var traceContext = _traceContextAccessor.Context.Refresh();

	        var clientTrace = new ClientTrace(
		        traceContext, 
		        request.Method.ToString(), 
		        remoteEndpoint: new Endpoint
		        {
			        ServiceName = _applicationName
		        });

	        // Add X-B3 headers to the outgoing request
	        request = _propagator.Inject(request, traceContext);

			return await SendAsync(request, cancellationToken, clientTrace);
        }

        private async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken, Trace trace)
        {
            trace.Start();

            try
            {
                var result = await base.SendAsync(request, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                trace.Error(ex.Message);
                throw;
            }
            finally
            {
                trace.End();
                _reporter.Report(trace.Span);
            }
        }
    }
}
