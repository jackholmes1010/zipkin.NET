using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Middleware
{
    public class TracingMiddleware : IMiddleware
    {
        private readonly string _applicationName;
        private readonly IExtractor<HttpRequest> _extractor;
        private readonly ITraceAccessor _traceAccessor;
        private readonly IReporter _reporter;
        private readonly ISampler _sampler;

        public TracingMiddleware(
            string applicationName,
            IExtractor<HttpRequest> extractor,
            ITraceAccessor traceAccessor,
            IReporter reporter,
            ISampler sampler)
        {
            _applicationName = applicationName;
            _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var trace = _extractor
                .Extract(context.Request)
                .Sample(_sampler);

            var spanBuilder = trace.GetSpanBuilder();

            spanBuilder
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
                await next(context);
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
            }
            finally
            {
                spanBuilder.End();
                
                if (trace.Sampled == true)
                    _reporter.Report(spanBuilder.Build());
            }
        }
    }
}
