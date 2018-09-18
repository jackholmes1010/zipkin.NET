using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;

namespace Zipkin.NET
{
    public class TracingHandler : DelegatingHandler
    {
        private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ITraceAccessor _traceAccessor;
        private readonly IPropagator<HttpRequestMessage> _propagator;

        public TracingHandler(HttpMessageHandler innerHandler,
            string applicationName,
            IReporter reporter, 
            ITraceAccessor traceAccessor,
            IPropagator<HttpRequestMessage> propagator) : base(innerHandler)
        {
            _applicationName = applicationName;
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
            _propagator = propagator ?? throw new ArgumentNullException(nameof(propagator));
        }

        public TracingHandler(
            string applicationName,
            IReporter reporter,
            ITraceAccessor traceAccessor,
            IPropagator<HttpRequestMessage> propagator)
        {
            _applicationName = applicationName;
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
            _propagator = propagator ?? throw new ArgumentNullException(nameof(propagator));
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var trace = _traceAccessor.HasTrace()
                ? _traceAccessor.GetTrace()
                : new Trace();

            var spanBuilder = trace
                .GetSpanBuilder()
                .Tag("uri", request.RequestUri.OriginalString)
                .Tag("method", request.Method.Method)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Add X-B3 headers to the request
            request = _propagator.Inject(request, spanBuilder.Build());

            spanBuilder.Start();

            try
            {
                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
                throw;
            }
            finally
            {
                spanBuilder.End();
                _reporter.Report(spanBuilder.Build());
            }
        }
    }
}
