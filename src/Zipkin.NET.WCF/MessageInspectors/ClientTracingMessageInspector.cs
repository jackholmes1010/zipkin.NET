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
        private readonly string _remoteServiceName;

        private readonly ISampler _sampler;
        private readonly IDispatcher _dispatcher;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly Propagator<HttpRequestMessageProperty> _propagator;

        public ClientTracingMessageInspector(
            string remoteServiceName,
            ISampler sampler,
            IDispatcher dispatcher,
            ITraceContextAccessor traceContextAccessor)
        {
            _remoteServiceName = remoteServiceName;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _traceContextAccessor = traceContextAccessor;
            _propagator = new HttpRequestMessagePropertyB3Propagator();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var traceContext = _traceContextAccessor.HasTrace()
                ? _traceContextAccessor
                    .GetTrace()
                    .Refresh()
                : new TraceContext();

            traceContext.Sample(_sampler);

            var httpRequest = ExtractHttpRequest(request);

            traceContext.SpanBuilder
                .Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _remoteServiceName
                });

            // Inject X-B3 headers to the outgoing request
            _propagator.Propagate(httpRequest, traceContext);

            _traceContextAccessor.SaveTrace(traceContext);

            return traceContext;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var traceContext = (TraceContext)correlationState;
            var span = traceContext.SpanBuilder
                .End()
                .Build();

            _dispatcher.Dispatch(span, traceContext);
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
