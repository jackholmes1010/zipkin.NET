using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Core.Propagation;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// .NET Core middleware used to build and report server spans from incoming requests.
    /// </summary>
    public class TracingMiddleware : IMiddleware
    {
        private readonly string _applicationName;
        private readonly IExtractor<HttpRequest> _extractor;

        public TracingMiddleware(string applicationName)
        {
            _applicationName = applicationName;
            _extractor = new HttpRequestExtractor();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var traceContext = _extractor.Extract(context.Request);

            Tracer.Sampler.Sample(ref traceContext);

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
                    ServiceName = _applicationName
                });

            Tracer.ContextAccessor.SaveTrace(traceContext);

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
                Tracer.Report(traceContext, spanBuilder.Build());
            }
        }
    }
}
