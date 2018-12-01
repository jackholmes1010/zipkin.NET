using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Exceptions;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.OWIN
{
    /// <summary>
    /// OWIN middleware used to build and report server spans from incoming requests.
    /// </summary>
    public class TracingMiddleware : OwinMiddleware
    {
        private readonly string _localEndpointName;
        private readonly ISpanContextAccessor _spanContextAccessor;
        private readonly IDispatcher _dispatcher;
        private readonly ISampler _sampler;
        private readonly ISpanContextExtractor<IOwinContext> _spanContextExtractor;

        /// <summary>
        /// Construct a new <see cref="TracingMiddleware"/>.
        /// </summary>
        /// <param name="next">
        /// The next middleware in the pipeline.
        /// </param>
        /// <param name="localEndpointName">
        /// The endpoint name describes the host recording the span.
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
        public TracingMiddleware(
            OwinMiddleware next,
            string localEndpointName,
            ISpanContextAccessor spanContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler) : base(next)
        {
            _localEndpointName = localEndpointName;
            _spanContextAccessor = spanContextAccessor ?? throw new ArgumentNullException(nameof(spanContextAccessor));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            _spanContextExtractor = new OwinContextB3SpanContextExtractor();
        }

        public override async Task Invoke(IOwinContext context)
        {
            var spanContext = _spanContextExtractor
                .Extract(context)
                .Sample(_sampler);

            var spanBuilder = new SpanBuilder(spanContext);
            spanBuilder.Start()
                .Name(context.Request.Method)
                .Kind(SpanKind.Server)
                .Tag("host", context.Request.Host.Value)
                .Tag("resource", context.Request.Path.Value)
                .Tag("method", context.Request.Method)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _localEndpointName
                });

            _spanContextAccessor.SaveContext(spanContext);

            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
            }
            finally
            {
                var span = spanBuilder
                    .End()
                    .Build();

                try
                {
                    _dispatcher.Dispatch(span);
                }
                catch (DispatchException)
                {
                    // ignore
                }
            }
        }
    }
}
