using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    public class TracingHandler : DelegatingHandler
    {
        private readonly string _applicationName;
        private readonly ISampler _sampler;
        private readonly ITraceAccessor _traceAccessor;
        private readonly IPropagator<HttpRequestMessage> _propagator;

        public TracingHandler(HttpMessageHandler innerHandler,
            string applicationName,
            ISampler sampler,
            ITraceAccessor traceAccessor,
            IPropagator<HttpRequestMessage> propagator) : base(innerHandler)
        {
            _applicationName = applicationName;
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
            _propagator = propagator ?? throw new ArgumentNullException(nameof(propagator));
        }

        public TracingHandler(
            string applicationName,
            ISampler sampler,
            ITraceAccessor traceAccessor,
            IPropagator<HttpRequestMessage> propagator)
        {
            _applicationName = applicationName;
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
            _propagator = propagator ?? throw new ArgumentNullException(nameof(propagator));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var trace = (_traceAccessor.HasTrace()
                    ? _traceAccessor.GetTrace().Refresh()
                    : new TraceContext())
                .Sample(_sampler);

            var spanBuilder = trace
                .GetSpanBuilder()
                .Start()
                .Name(request.Method.Method)
                .Kind(SpanKind.Client)
                .Tag("uri", request.RequestUri.OriginalString)
                .Tag("method", request.Method.Method)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Add X-B3 headers to the request
            request = _propagator.Inject(request, spanBuilder.Build(), trace.Sampled == true);

            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
                throw;
            }
            finally
            {
                spanBuilder.End();

                if (trace.Sampled == true)
                    TraceManager.Report(spanBuilder.Build());
            }
        }
    }
}
