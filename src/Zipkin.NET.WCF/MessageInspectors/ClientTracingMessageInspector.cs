using System;
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
        private readonly Func<ITraceContextAccessor> _getTraceContextAccessor;
        private readonly Func<ISampler> _getSampler;
        private readonly Func<IDispatcher> _getDispatcher;
        private readonly Propagator<HttpRequestMessageProperty> _propagator;

        public ClientTracingMessageInspector(
            string applicationName,
            Func<ITraceContextAccessor> getTraceContextAccessor,
            Func<ISampler> getSampler,
            Func<IDispatcher> getDispatcher)
        {
            _applicationName = applicationName;
            _getTraceContextAccessor = getTraceContextAccessor;
            _getSampler = getSampler;
            _getDispatcher = getDispatcher;
            _propagator = new HttpRequestMessagePropertyB3Propagator();
        }

        public ITraceContextAccessor TraceContextAccessor => _getTraceContextAccessor();

        public ISampler Sampler => _getSampler();

        public IDispatcher Dispatcher => _getDispatcher();

        // IClientMessageInspector
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var traceContext = TraceContextAccessor.HasTrace()
                ? TraceContextAccessor
                    .GetTrace()
                    .Refresh()
                : new TraceContext();

            traceContext.Sample(Sampler);

            var httpRequest = ExtractHttpRequest(request);

            traceContext.SpanBuilder
                .Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Inject X-B3 headers to the outgoing request
            _propagator.Propagate(httpRequest, traceContext);

            TraceContextAccessor.SaveTrace(traceContext);

            return null;
        }

        // IClientMessageInspector
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var traceContext = TraceContextAccessor.GetTrace();
            var span = traceContext.SpanBuilder
                .End()
                .Build();

            Dispatcher.Dispatch(span, traceContext);
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
