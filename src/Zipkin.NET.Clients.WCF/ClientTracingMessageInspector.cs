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
        private readonly Func<ITraceContextAccessor> _getTraceContextAccessor;
        private readonly Func<ISampler> _getSampler;
        private readonly Func<IDispatcher> _getDispatcher;
        private readonly Propagator<HttpRequestMessageProperty> _propagator;

        /// <summary>
        /// Construct a new <see cref="ClientTracingMessageInspector"/>.
        /// </summary>
        /// <param name="remoteServiceName">
        /// The name of the WCF service the client is calling.
        /// </param>
        /// <param name="getTraceContextAccessor">
        /// Gets a <see cref="ITraceContextAccessor"/> used to access trace context.
        /// </param>
        /// <param name="getSampler">
        /// Gets a <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="getDispatcher">
        /// Gets a <see cref="IDispatcher"/> used to dispatch completed spans to reporters.
        /// </param>
        public ClientTracingMessageInspector(
            string remoteServiceName,
            Func<ITraceContextAccessor> getTraceContextAccessor,
            Func<ISampler> getSampler,
            Func<IDispatcher> getDispatcher)
        {
            _remoteServiceName = remoteServiceName;
            _getTraceContextAccessor = getTraceContextAccessor;
            _getSampler = getSampler;
            _getDispatcher = getDispatcher;
            _propagator = new HttpRequestMessagePropertyB3Propagator();
        }

        public ITraceContextAccessor TraceContextAccessor => _getTraceContextAccessor();

        public ISampler Sampler => _getSampler();

        public IDispatcher Dispatcher => _getDispatcher();

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
                    ServiceName = _remoteServiceName
                });

            // Inject X-B3 headers to the outgoing request
            _propagator.Propagate(httpRequest, traceContext);

            TraceContextAccessor.SaveTrace(traceContext);

            return null;
        }

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
