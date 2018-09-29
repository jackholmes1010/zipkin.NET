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
        private readonly Dispatcher _dispatcher;
        private readonly Sampler _sampler;
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
        /// A <see cref="Dispatcher"/> used to dispatch completed spans to reporters.
        /// </param>
        /// <param name="sampler">
        /// A <see cref="Sampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="remoteEndpointName">
        /// The name of the reciever.
        /// </param>
        public TracingHandler(
            HttpMessageHandler innerHandler,
            ITraceContextAccessor traceContextAccessor,
            Dispatcher dispatcher,
            Sampler sampler,
            string remoteEndpointName) : base(innerHandler)
        {
            _remoteEndpointName = remoteEndpointName;
            _traceContextAccessor = traceContextAccessor;
            _dispatcher = dispatcher;
            _sampler = sampler;
            _propagator = new HttpRequestMessagePropagator();
        }

        /// <summary>
        /// Construct a new <see cref="TracingHandler"/> with an inner handler.
        /// </summary>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context.
        /// </param>
        /// <param name="dispatcher">
        /// A <see cref="Dispatcher"/> used to dispatch completed spans to reporters.
        /// </param>
        /// <param name="sampler">
        /// A <see cref="Sampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="remoteEndpointName">
        /// The name of the reciever.
        /// </param>
        public TracingHandler(
            ITraceContextAccessor traceContextAccessor,
            Dispatcher dispatcher,
            Sampler sampler,
            string remoteEndpointName)
        {
            _remoteEndpointName = remoteEndpointName;
            _traceContextAccessor = traceContextAccessor;
            _dispatcher = dispatcher;
            _sampler = sampler;
            _propagator = new HttpRequestMessagePropagator();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            TraceContext traceContext;

            if (!_traceContextAccessor.HasTrace())
            {
                traceContext = new TraceContext();
                _traceContextAccessor.SaveTrace(traceContext);
            }
            else
            {
                traceContext = _traceContextAccessor
                    .GetTrace()
                    .Refresh();
            }

            traceContext.Sample(_sampler);

            var spanBuilder = traceContext
                .GetSpanBuilder()
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
                spanBuilder.End();
                _dispatcher.Dispatch(spanBuilder.Build());
            }
        }
    }
}
