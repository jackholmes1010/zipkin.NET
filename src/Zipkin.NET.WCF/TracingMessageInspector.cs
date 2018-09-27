using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.WCF
{
    public class TracingMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        private readonly string _applicationName;
        private readonly IPropagator<HttpRequestMessageProperty> _propagator;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        private TraceContext _clientTraceContext;
        private TraceContext _serverTraceContext;
        private SpanBuilder _spanBuilder;

        public TracingMessageInspector(string applicationName)
        {
            _applicationName = applicationName;
            _propagator = new HttpRequestMessagePropertyB3Propagator();
            _extractor = new IncomingWebRequestB3Extractor();
        }

        // IClientMessageInspector
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _clientTraceContext = Tracer.ContextAccessor.HasTrace()
                ? Tracer.ContextAccessor.GetTrace().Refresh()
                : new TraceContext();
            
            Tracer.Sample(ref _clientTraceContext);

            var httpRequest = ExtractHttpRequest(request);

            _spanBuilder = _clientTraceContext
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
                httpRequest, _spanBuilder.Build(), _clientTraceContext.Sampled == true);

            return null;
        }

        // IClientMessageInspector
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            _spanBuilder.End();
            Tracer.Report(_clientTraceContext, _spanBuilder.Build());
        }

        // IDispatchMessageInspector
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var traceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            Tracer.Sample(ref traceContext);

            var spanBuilder = traceContext
                .GetSpanBuilder()
                .Start()
                .Tag("action", request.Headers.Action)
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });
                
            Tracer.ContextAccessor.SaveTrace(traceContext);

            _serverTraceContext = traceContext;
            _spanBuilder = spanBuilder;

            return request;
        }

        // IDispatchMessageInspector
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            _spanBuilder.End();

            Tracer.Report(_serverTraceContext, _spanBuilder.Build());
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
