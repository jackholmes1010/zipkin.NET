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
        private readonly string _localEndpointName;
        private readonly IExtractor<IOwinContext> _extractor;

        /// <summary>
        /// Construct a new <see cref="TracingMiddleware"/>.
        /// </summary>
        /// <param name="localEndpointName">
        /// The endpoint name describes the host recording the span.
        /// </param>
        public TracingMiddleware(string localEndpointName)
        {
            _localEndpointName = localEndpointName;
            _extractor = new OwinContextB3Extractor();
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            var traceContext = _extractor
                .Extract(context)
                .Sample();

            var spanBuilder = traceContext
                .GetSpanBuilder()
                .Start()
                .Name(context.Request.Method)
                .Kind(SpanKind.Server)
                .Tag("host", context.Request.Host.Value)
                .Tag("resource", context.Request.Path.Value)
                .Tag("method", context.Request.Method)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _localEndpointName
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
