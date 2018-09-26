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
        private readonly IExtractor<IOwinContext> _extractor;

        public TracingMiddleware(
            string applicationName)
        {
            _applicationName = applicationName;
            _extractor = new OwinContextB3Extractor();
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

            Tracer.ContextAccessor.SaveTrace(traceContext);

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
