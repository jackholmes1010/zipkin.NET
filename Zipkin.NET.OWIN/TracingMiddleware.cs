using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.OWIN
{
    public class TracingMiddleware
    {
        private readonly string _applicationName;
        private readonly ISampler _sampler;
        private readonly ITraceAccessor _traceAccessor;
        private readonly IExtractor<IOwinContext> _extractor;

        public TracingMiddleware(
            string applicationName,
            ISampler sampler,
            ITraceAccessor traceAccessor,
            IExtractor<IOwinContext> extractor)
        {
            _applicationName = applicationName;
            _sampler = sampler;
            _traceAccessor = traceAccessor;
            _extractor = extractor;
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            var trace = _extractor
                .Extract(context)
                .Sample(_sampler);

            var spanBuilder = trace.GetSpanBuilder();

            spanBuilder
                .Kind(SpanKind.Server)
                .Tag("host", context.Request.Host.Value)
                .Tag("resource", context.Request.Path.Value)
                .Tag("method", context.Request.Method)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                })
                .Start();

            _traceAccessor.SaveTrace(trace);

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
                
                if (trace.Sampled == true)
                    TraceManager.Report(spanBuilder.Build());
            }
        }
    }
}
