using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;
using Zipkin.NET.WCF.Propagation;

namespace Zipkin.NET.WCF.MessageInspectors
{
    public class DispatchTracingMessageInspector :  IDispatchMessageInspector
    {
        private readonly string _localEndpointName;
        private readonly ISampler _sampler;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IDispatcher _dispatcher;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        public DispatchTracingMessageInspector(string localEndpointName, string zipkinHost, float sampleRate)
        {
            _localEndpointName = localEndpointName;
            _sampler = new RateSampler(sampleRate);
            _traceContextAccessor = new SystemWebHttpContextTraceContextAccessor();
            _extractor = new IncomingWebRequestB3Extractor();

            var sender = new ZipkinHttpSender(zipkinHost);
            var reporter = new ZipkinReporter(sender);
            var logger = new ConsoleInstrumentationLogger();
            _dispatcher = new AsyncActionBlockDispatcher(new[] { reporter }, logger);
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var traceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            traceContext.Sample(_sampler);

            traceContext.SpanBuilder
                .Start()
                .Tag("action", request.Headers.Action)
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _localEndpointName
                });
                
            _traceContextAccessor.SaveTrace(traceContext);

            return traceContext;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var traceContext = (TraceContext) correlationState;
            var span = traceContext.SpanBuilder
                .End()
                .Build();

            _dispatcher.Dispatch(span, traceContext);
        }
    }
}
