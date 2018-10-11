using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Message inspector used by WCF clients to build and 
    /// report client spans from outgoing WCF client requests.
    /// <remarks>
    /// This message inspector is supported by .NET Standard.
    /// </remarks>
    /// </summary>
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

            // Inject X-B3 headers to the outgoing request
            _propagator.Propagate(httpRequest, traceContext);

            var spanBuilder = traceContext.SpanBuilder
                .Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _remoteServiceName
                });

            return new Tuple<TraceContext, SpanBuilder>(traceContext, spanBuilder);
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var context = (Tuple<TraceContext, SpanBuilder>) correlationState;
            var span = context.Item2
                .End()
                .Build();

            _dispatcher.Dispatch(span, context.Item1);
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
