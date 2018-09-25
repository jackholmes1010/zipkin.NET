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
        private readonly ITraceAccessor _traceAccessor;
        private readonly IPropagator<HttpRequestMessageProperty> _propagator;

        private SpanBuilder _spanBuilder;
        private TraceContext _traceContext;

        public TracingMessageInspector(
            string applicationName,
            ITraceAccessor traceAccessor,
            IPropagator<HttpRequestMessageProperty> propagator)
        {
            _applicationName = applicationName;
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
            _propagator = propagator ?? throw new ArgumentNullException(nameof(traceAccessor));
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _traceContext = _traceAccessor.HasTrace()
                ? _traceAccessor.GetTrace().Refresh()
                : new TraceContext();
            
            TraceManager.Sample(ref _traceContext);

            var httpRequest = ExtractHttpRequest(request);

            _spanBuilder = _traceContext
                .GetSpanBuilder()
                .Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Inject X-B3 headers to the outgoing request
            _propagator.Inject(
                httpRequest, _spanBuilder.Build(), _traceContext.Sampled == true);

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            _spanBuilder.End();
            TraceManager.Report(_traceContext, _spanBuilder.Build());
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
