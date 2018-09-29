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
    public class TracingMessageInspector : IClientMessageInspector
    {
        private readonly string _remoteServiceName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly Sampler _sampler;
        private readonly Dispatcher _dispatcher;
        private readonly Propagator<HttpRequestMessageProperty> _propagator;

        private SpanBuilder _spanBuilder;
        private TraceContext _traceContext;

        /// <summary>
        /// Construct a new <see cref="TracingMessageInspector"/>.
        /// </summary>
        /// <param name="remoteServiceName">
        /// The name of the WCF service the client is calling.
        /// </param>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context.
        /// </param>
        /// <param name="sampler">
        /// A <see cref="Sampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="dispatcher">
        /// A <see cref="Dispatcher"/> used to dispatch completed spans to reporters.
        /// </param>
        public TracingMessageInspector(
            string remoteServiceName,
            ITraceContextAccessor traceContextAccessor,
            Sampler sampler,
            Dispatcher dispatcher)
        {
            _remoteServiceName = remoteServiceName;
            _traceContextAccessor = traceContextAccessor;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _propagator = new HttpRequestMessagePropertyB3Propagator();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _traceContext = _traceContextAccessor.HasTrace()
                ? _traceContextAccessor.GetTrace().Refresh()
                : new TraceContext();

            _traceContext.Sample(_sampler);
            _traceContextAccessor.SaveTrace(_traceContext);

            var httpRequest = ExtractHttpRequest(request);

            _spanBuilder = _traceContext
                .GetSpanBuilder()
                .Start()
                .Kind(SpanKind.Client)
                .Tag("action", request.Headers.Action)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _remoteServiceName
                });

            // Inject X-B3 headers to the outgoing request
            _propagator.Propagate(httpRequest, _traceContext);

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var span = _spanBuilder
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
