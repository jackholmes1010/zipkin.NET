using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Middleware
{
    /// <summary>
    /// .NET Core middleware used to build and report server spans from incoming requests.
    /// </summary>
    public class TracingMiddleware : IMiddleware
    {
        private readonly string _applicationName;
        private readonly IExtractor<HttpRequest> _extractor;
        private readonly ITraceAccessor _traceAccessor;

        public TracingMiddleware(
            string applicationName,
            IExtractor<HttpRequest> extractor,
            ITraceAccessor traceAccessor)
        {
            _applicationName = applicationName;
            _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var traceContext = _extractor.Extract(context.Request);

            TraceManager.Sample(ref traceContext);

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

            _traceAccessor.SaveTrace(traceContext);

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
                TraceManager.Report(traceContext, spanBuilder.Build());
            }
        }
    }
}
