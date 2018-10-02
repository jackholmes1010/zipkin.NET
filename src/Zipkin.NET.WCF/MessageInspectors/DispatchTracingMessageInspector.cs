﻿using System.ServiceModel;
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
        private readonly string _applicationName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly ISampler _sampler;
        private readonly IDispatcher _dispatcher;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        private TraceContext _serverTraceContext;
        private SpanBuilder _serverSpanBuilder;

        public DispatchTracingMessageInspector(
            string applicationName,
            ITraceContextAccessor traceContextAccessor,
            ISampler sampler,
            IDispatcher dispatcher)
        {
            _applicationName = applicationName;
            _traceContextAccessor = traceContextAccessor;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _extractor = new IncomingWebRequestB3Extractor();
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

            _dispatcher.Dispatch(span, _serverTraceContext);
        }
    }
}
