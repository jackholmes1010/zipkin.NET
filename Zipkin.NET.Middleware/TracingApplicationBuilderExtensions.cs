using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zipkin.NET.Logging;
using Zipkin.NET.Middleware.Logging;
using Zipkin.NET.Middleware.TraceAccessors;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;

namespace Zipkin.NET.Middleware
{
    public static class TracingApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTracingMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<TracingMiddleware>();
        }

        public static IApplicationBuilder UseZipkinTracer(this IApplicationBuilder app,
            string zipkinHost = "http://localhost:9411",
            Sampler sampler = null,
            ISender sender = null,
            ITraceContextAccessor traceContextAccessor = null,
            IInstrumentationLogger instrumentationLogger = null)
        {
            var reporter = new ZipkinReporter(sender ?? new HttpSender(zipkinHost));

            app.UseTracer(
                new[] {reporter},
                sampler,
                traceContextAccessor,
                instrumentationLogger);

            return app;
        }

        public static IApplicationBuilder UseTracer(this IApplicationBuilder app,
            IEnumerable<IReporter> reporters,
            Sampler sampler = null,
            ITraceContextAccessor traceContextAccessor = null,
            IInstrumentationLogger instrumentationLogger = null)
        {
            if (sampler == null)
            {
                sampler = new DebugSampler();
            }

            if (traceContextAccessor == null)
            {
                var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();
                traceContextAccessor = new HttpContextTraceContextAccessor(httpContextAccessor);
            }

            if (instrumentationLogger == null)
            {
                var logger = app.ApplicationServices.GetService<ILogger<CoreInstrumentationLogger>>();
                instrumentationLogger = new CoreInstrumentationLogger(logger);
            }

            Tracer.Start(
                sampler,
                traceContextAccessor,
                instrumentationLogger,
                reporters);

            return app;
        }
    }
}
