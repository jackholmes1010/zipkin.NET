using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.OWIN
{
    /// <summary>
    /// OWIN middleware used to build and report server spans from incoming requests.
    /// </summary>
    public class TracingMiddleware
    {
        private readonly string _applicationName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IExtractor<IOwinContext> _extractor;

        public TracingMiddleware(
            string applicationName,
            ITraceContextAccessor traceContextAccessor,
            IExtractor<IOwinContext> extractor)
        {
            _applicationName = applicationName;
            _traceContextAccessor = traceContextAccessor ?? throw new ArgumentNullException(nameof(traceContextAccessor));
            _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            var traceContext = _extractor.Extract(context);

            Tracer.Sampler.Sample(ref traceContext);

            var spanBuilder = traceContext.GetSpanBuilder();

            spanBuilder
                .Start()
                .Name(context.Request.Method)
                .Kind(SpanKind.Server)
                .Tag("host", context.Request.Host.Value)
                .Tag("resource", context.Request.Path.Value)
                .Tag("method", context.Request.Method)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });

            _traceContextAccessor.SaveTrace(traceContext);

            try
            {
                await next();
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
            }
            finally
            {
                spanBuilder.End();
                Tracer.Report(traceContext, spanBuilder.Build());
            }
        }
    }
}
