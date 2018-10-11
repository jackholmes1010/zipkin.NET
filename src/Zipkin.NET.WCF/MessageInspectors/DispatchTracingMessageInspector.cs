using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;
using Zipkin.NET.WCF.Propagation;

namespace Zipkin.NET.WCF.MessageInspectors
{
    public class DispatchTracingMessageInspector :  IDispatchMessageInspector
    {
        private readonly string _localEndpointName;
        private readonly ISampler _sampler;
        private readonly IDispatcher _dispatcher;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        public DispatchTracingMessageInspector(
            string localEndpointName,
            ISampler sampler,
            IDispatcher dispatcher,
            ITraceContextAccessor traceContextAccessor)
        {
            _localEndpointName = localEndpointName;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _traceContextAccessor = traceContextAccessor;
            _extractor = new IncomingWebRequestB3Extractor();
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var traceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            traceContext.Sample(_sampler);

            var spanBuilder = traceContext.SpanBuilder
                .Start()
                .Tag("action", request.Headers.Action)
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _localEndpointName
                });
                
            _traceContextAccessor.SaveTrace(traceContext);

            return new Tuple<TraceContext, SpanBuilder>(traceContext, spanBuilder);
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var context = (Tuple<TraceContext, SpanBuilder>)correlationState;
            var span = context.Item2
                .End()
                .Build();

            _dispatcher.Dispatch(span, context.Item1);
        }
    }
}
