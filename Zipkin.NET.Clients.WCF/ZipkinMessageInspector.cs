using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Clients.WCF
{
    public class ZipkinMessageInspector : IClientMessageInspector
    {
        private readonly string _applicationName;
        private readonly ITraceAccessor _traceAccessor;
        private readonly IPropagator<HttpRequestMessageProperty> _propagator;

        private SpanBuilder _spanBuilder;
        private bool _sampled;

        public ZipkinMessageInspector(
            string applicationName,
            ITraceAccessor traceAccessor,
            IPropagator<HttpRequestMessageProperty> propagator)
        {
            _applicationName = applicationName;
            _traceAccessor = traceAccessor;
            _propagator = propagator;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var trace = _traceAccessor.HasTrace()
                ? _traceAccessor.GetTrace().Refresh()
                : new TraceContext();

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
