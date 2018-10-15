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
        private readonly ISpanContextAccessor _spanContextAccessor;
        private readonly ISpanContextInjector<HttpRequestMessageProperty> _spanContextInjector;

        public ClientTracingMessageInspector(
            string remoteServiceName,
            ISampler sampler,
            IDispatcher dispatcher,
            ISpanContextAccessor spanContextAccessor)
        {
            _remoteServiceName = remoteServiceName;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _spanContextAccessor = spanContextAccessor;
            _spanContextInjector = new HttpRequestMessagePropertyB3SpanContextInjector();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var spanContext = _spanContextAccessor.HasContext()
                ? _spanContextAccessor
                    .GetContext()
                    .CreateChild()
                : new SpanContext();

            spanContext.Sample(_sampler);

            var httpRequest = ExtractHttpRequest(request);

              // Inject X-B3 headers to the outgoing request
            _spanContextInjector.Inject(httpRequest, spanContext);

            var spanBuilder = new SpanBuilder(spanContext);
            spanBuilder.Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _remoteServiceName
                });

            return spanBuilder;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var spanBuilder = (SpanBuilder)correlationState;
            var span = spanBuilder
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
