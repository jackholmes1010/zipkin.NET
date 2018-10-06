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
        private readonly Func<ITraceContextAccessor> _getTraceContextAccessor;
        private readonly Func<ISampler> _getSampler;
        private readonly Func<IDispatcher> _getDispatcher;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        public DispatchTracingMessageInspector(
            string localEndpointName,
            Func<ITraceContextAccessor> getTraceContextAccessor,
            Func<ISampler> getSampler,
            Func<IDispatcher> getDispatcher)
        {
            _localEndpointName = localEndpointName;
            _getTraceContextAccessor = getTraceContextAccessor;
            _getSampler = getSampler;
            _getDispatcher = getDispatcher;
            _extractor = new IncomingWebRequestB3Extractor();
        }

        public ITraceContextAccessor TraceContextAccessor => _getTraceContextAccessor();

        public ISampler Sampler => _getSampler();

        public IDispatcher Dispatcher => _getDispatcher();

        // IDispatchMessageInspector
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var traceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            traceContext.Sample(Sampler);

            traceContext.SpanBuilder
                .Start()
                .Tag("action", request.Headers.Action)
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _localEndpointName
                });
                
            TraceContextAccessor.SaveTrace(traceContext);

            return request;
        }

        // IDispatchMessageInspector
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var traceContext = TraceContextAccessor.GetTrace();
            var span = traceContext.SpanBuilder
                .End()
                .Build();

            Dispatcher.Dispatch(span, traceContext);
        }
    }
}
