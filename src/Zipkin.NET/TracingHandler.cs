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
    /// report client spans and propagate span context.
    /// </summary>
    public class TracingHandler : DelegatingHandler
    {
        private readonly string _remoteEndpointName;
        private readonly ISpanContextAccessor _spanContextAccessor;
        private readonly IDispatcher _dispatcher;
        private readonly ISampler _sampler;
        private readonly Propagator<HttpRequestMessage> _propagator;

        /// <summary>
        /// Construct a new <see cref="TracingHandler"/> with an inner handler.
        /// </summary>
        /// <param name="innerHandler">
        /// An optional inner handler.
        /// </param>
        /// <param name="spanContextAccessor">
        /// A <see cref="ISpanContextAccessor"/> used to access the parent span context.
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
            ISpanContextAccessor spanContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler,
            string remoteEndpointName) 
            : base(innerHandler)
        {
            _remoteEndpointName = remoteEndpointName ?? throw new ArgumentNullException(nameof(remoteEndpointName));
            _spanContextAccessor = spanContextAccessor ?? throw new ArgumentNullException(nameof(spanContextAccessor));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _propagator = new HttpRequestMessagePropagator();
        }

        /// <summary>
        /// Construct a new <see cref="TracingHandler"/> with an inner handler.
        /// </summary>
        /// <param name="spanContextAccessor">
        /// A <see cref="ISpanContextAccessor"/> used to access span context.
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
            ISpanContextAccessor spanContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler,
            string remoteEndpointName)
        {
            _remoteEndpointName = remoteEndpointName ?? throw new ArgumentNullException(nameof(remoteEndpointName));
            _spanContextAccessor = spanContextAccessor ?? throw new ArgumentNullException(nameof(spanContextAccessor));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _propagator = new HttpRequestMessagePropagator();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var spanContext = _spanContextAccessor.HasContext()
                ? _spanContextAccessor
                    .GetContext()
                    .CreateChild()
                : new SpanContext();

            spanContext.Sample(_sampler);

            var spanBuilder = new SpanBuilder(spanContext);
            spanBuilder.Start()
                .Name(request.Method.Method)
                .Kind(SpanKind.Client)
                .Tag("uri", request.RequestUri.OriginalString)
                .Tag("method", request.Method.Method)
                .WithRemoteEndpoint(new Endpoint
                {
                    ServiceName = _remoteEndpointName
                });

            // Add X-B3 headers to the request
            request = _propagator.Propagate(request, spanContext);

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

                _dispatcher.Dispatch(span);
            }
        }
    }
}
