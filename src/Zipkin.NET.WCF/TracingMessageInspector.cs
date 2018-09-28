﻿using System.ServiceModel;
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
        private readonly Propagator<HttpRequestMessageProperty> _propagator;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        private TraceContext _clientTraceContext;
        private TraceContext _serverTraceContext;
        private SpanBuilder _clientSpanBuilder;
        private SpanBuilder _serverSpanBuilder;

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

            _clientTraceContext.Sample();

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
            _clientSpanBuilder.End();
            Tracer.Report(_clientTraceContext, _clientSpanBuilder.Build());
        }

        // IDispatchMessageInspector
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            _serverTraceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            _serverTraceContext.Sample();

            _serverSpanBuilder = _serverTraceContext
                .GetSpanBuilder()
                .Start()
                .Tag("action", request.Headers.Action)
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });
                
            Tracer.ContextAccessor.SaveTrace(_serverTraceContext);

            return request;
        }

        // IDispatchMessageInspector
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            _serverSpanBuilder.End();

            Tracer.Report(_serverTraceContext, _serverSpanBuilder.Build());
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
