using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;
using Zipkin.NET.WCF.Propagation;

namespace Zipkin.NET.WCF.MessageInspectors
{
    public class ClientTracingMessageInspector : IClientMessageInspector
    {
        private readonly string _applicationName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly Sampler _sampler;
        private readonly Dispatcher _dispatcher;
        private readonly Propagator<HttpRequestMessageProperty> _propagator;

        private TraceContext _clientTraceContext;
        private SpanBuilder _clientSpanBuilder;

        public ClientTracingMessageInspector(
            string applicationName,
            ITraceContextAccessor traceContextAccessor,
            Sampler sampler,
            Dispatcher dispatcher)
        {
            _applicationName = applicationName;
            _traceContextAccessor = traceContextAccessor;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _propagator = new HttpRequestMessagePropertyB3Propagator();
        }

        // IClientMessageInspector
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (!_traceContextAccessor.HasTrace())
            {
                _clientTraceContext = new TraceContext();
                _traceContextAccessor.SaveTrace(_clientTraceContext);
            }
            else
            {
                _clientTraceContext = _traceContextAccessor
                    .GetTrace()
                    .Refresh();
            }

            _clientTraceContext.Sample(_sampler);

            var httpRequest = ExtractHttpRequest(request);

            _clientSpanBuilder = _clientTraceContext
                .GetSpanBuilder()
                .Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Inject X-B3 headers to the outgoing request
            _propagator.Propagate(httpRequest, _clientTraceContext);

            return null;
        }

        // IClientMessageInspector
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var span = _clientSpanBuilder
                .End()
                .Build();

            _dispatcher.Dispatch(span);
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
