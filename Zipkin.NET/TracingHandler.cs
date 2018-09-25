using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET
{
    /// <summary>
    /// Delegating handler used by http clients to build
    /// and report client spans from outgoing requests.
    /// </summary>
    public class TracingHandler : DelegatingHandler
    {
        private readonly string _applicationName;
        private readonly IPropagator<HttpRequestMessage> _propagator;

        public TracingHandler(
            HttpMessageHandler innerHandler,
            string applicationName) : base(innerHandler)
        {
            _applicationName = applicationName;
            _propagator = new HttpRequestMessagePropagator();
        }

        public TracingHandler(string applicationName)
        {
            _applicationName = applicationName;
            _propagator = new HttpRequestMessagePropagator();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var traceContext = Tracer.ContextAccessor.HasTrace()
                ? Tracer.ContextAccessor.GetTrace().Refresh()
                : new TraceContext();

            Tracer.Sampler.Sample(ref traceContext);

            var spanBuilder = traceContext
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
            request = _propagator.Inject(request, spanBuilder.Build(), traceContext.Sampled == true);

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
                Tracer.Report(traceContext, spanBuilder.Build());
            }
        }
    }
}
