using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.WCF
{
    public class TracingMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        private readonly string _applicationName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly Sampler _sampler;
        private readonly Dispatcher _dispatcher;
        private readonly Propagator<HttpRequestMessageProperty> _propagator;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        private TraceContext _clientTraceContext;
        private TraceContext _serverTraceContext;
        private SpanBuilder _clientSpanBuilder;
        private SpanBuilder _serverSpanBuilder;

        public TracingMessageInspector(
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
            _extractor = new IncomingWebRequestB3Extractor();
        }

        // IClientMessageInspector
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _clientTraceContext = _traceContextAccessor.HasTrace()
                ? _traceContextAccessor.GetTrace().Refresh()
                : new TraceContext();

            _clientTraceContext.Sample(_sampler);
            _traceContextAccessor.SaveTrace(_clientTraceContext);

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

        // IDispatchMessageInspector
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            _serverTraceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            _serverTraceContext.Sample(_sampler);

            _serverSpanBuilder = _serverTraceContext
                .GetSpanBuilder()
                .Start()
                .Tag("action", request.Headers.Action)
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });
                
            _traceContextAccessor.SaveTrace(_serverTraceContext);

            return request;
        }

        // IDispatchMessageInspector
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var span = _serverSpanBuilder
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
