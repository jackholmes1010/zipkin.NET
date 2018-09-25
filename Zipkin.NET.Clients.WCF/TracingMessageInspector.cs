using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Message inspector used by WCF clients to build
    /// and report client spans from outgoing requests.
    /// </summary>
    public class TracingMessageInspector : IClientMessageInspector
    {
        private readonly string _applicationName;
        private readonly ISampler _sampler;
        private readonly ITraceAccessor _traceAccessor;
        private readonly IPropagator<HttpRequestMessageProperty> _propagator;

        private SpanBuilder _spanBuilder;
        private bool _sampled;

        public TracingMessageInspector(
            string applicationName,
            ISampler sampler,
            ITraceAccessor traceAccessor,
            IPropagator<HttpRequestMessageProperty> propagator)
        {
            _applicationName = applicationName;
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
            _propagator = propagator ?? throw new ArgumentNullException(nameof(traceAccessor));
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var trace = (_traceAccessor.HasTrace()
                    ? _traceAccessor.GetTrace().Refresh()
                    : new TraceContext())
                .Sample(_sampler);

            _sampled = trace.Sampled == true;

            var httpRequest = ExtractHttpRequest(request);

            _spanBuilder = trace
                .GetSpanBuilder()
                .Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Inject X-B3 headers to the outgoing request
            _propagator.Inject(httpRequest, _spanBuilder.Build(), _sampled);

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            _spanBuilder.End();

            if (_sampled)
                TraceManager.Report(_spanBuilder.Build());
        }

        private static HttpRequestMessageProperty ExtractHttpRequest(Message wcfMessage)
        {
            HttpRequestMessageProperty httpRequest;
            if (wcfMessage.Properties.TryGetValue(HttpRequestMessageProperty.Name, out var requestObject))
            {
                httpRequest = requestObject as HttpRequestMessageProperty;
            }
            else
            {
                httpRequest = new HttpRequestMessageProperty();
                wcfMessage.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
            }

            return httpRequest;
        }
    }
}
