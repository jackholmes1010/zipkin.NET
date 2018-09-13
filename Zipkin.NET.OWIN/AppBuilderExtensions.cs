using Owin;
using System;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.OWIN
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseZipkin(this IAppBuilder app, string applicationName, string zipkinHost)
        {
            app.Use(async (ctx, next) =>
            {
                try
                {
                    var sender = new HttpSender(zipkinHost);
                    var reporter = new Reporter(sender);
                    var sampler = new DebugSampler();
                    var propagator = new B3Extractor();
                    var traceContextAccessor = new TraceContextAccessor();
                    var middleware = new ZipkinMiddleware(
                        applicationName, reporter, sampler, traceContextAccessor, propagator);
                    await middleware.Invoke(ctx, next);
                }
                catch (Exception ex)
                {

                }
            });

            return app;
        }
    }
}
