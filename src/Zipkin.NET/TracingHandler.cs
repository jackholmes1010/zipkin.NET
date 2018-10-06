using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    /// <summary>
    /// Delegating handler used by http clients to 
    /// report client spans and propagate trace context.
    /// </summary>
    public class TracingHandler : DelegatingHandler
    {
        private readonly string _remoteEndpointName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IDispatcher _dispatcher;
        private readonly ISampler _sampler;
        private readonly Propagator<HttpRequestMessage> _propagator;

        /// <summary>
        /// Construct a new <see cref="TracingHandler"/> with an inner handler.
        /// </summary>
        /// <param name="innerHandler">
        /// An optional inner handler.
        /// </param>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context.
        /// </param>
        /// <param name="dispatcher">
        /// A <see cref="IDispatcher"/> used to dispatch completed spans to reporters.
        /// </param>
        /// <param name="sampler">
        /// A <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="remoteEndpointName">
        /// The name of the receiver.
        /// </param>
        public TracingHandler(
            HttpMessageHandler innerHandler,
            ITraceContextAccessor traceContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler,
            string remoteEndpointName) 
            : base(innerHandler)
        {
            _remoteEndpointName = remoteEndpointName ?? throw new ArgumentNullException(nameof(remoteEndpointName));
            _traceContextAccessor = traceContextAccessor ?? throw new ArgumentNullException(nameof(traceContextAccessor));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _propagator = new HttpRequestMessagePropagator();
        }

        /// <summary>
        /// Construct a new <see cref="TracingHandler"/> with an inner handler.
        /// </summary>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context.
        /// </param>
        /// <param name="dispatcher">
        /// A <see cref="IDispatcher"/> used to dispatch completed spans to reporters.
        /// </param>
        /// <param name="sampler">
        /// A <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="remoteEndpointName">
        /// The name of the receiver.
        /// </param>
        public TracingHandler(
            ITraceContextAccessor traceContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler,
            string remoteEndpointName)
        {
            _remoteEndpointName = remoteEndpointName ?? throw new ArgumentNullException(nameof(remoteEndpointName));
            _traceContextAccessor = traceContextAccessor ?? throw new ArgumentNullException(nameof(traceContextAccessor));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _propagator = new HttpRequestMessagePropagator();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var traceContext = _traceContextAccessor.HasTrace()
                ? _traceContextAccessor
                    .GetTrace()
                    .Refresh()
                : new TraceContext();

            traceContext.Sample(_sampler);

            var spanBuilder = traceContext.SpanBuilder
                .Start()
                .Name(request.Method.Method)
                .Kind(SpanKind.Client)
                .Tag("uri", request.RequestUri.OriginalString)
                .Tag("method", request.Method.Method)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _remoteEndpointName
                });

            // Add X-B3 headers to the request
            request = _propagator.Propagate(request, traceContext);

            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
                throw;
            }
            finally
            {
                var span = spanBuilder
                    .End()
                    .Build();

                _dispatcher.Dispatch(span, traceContext);
            }
        }
    }
}
