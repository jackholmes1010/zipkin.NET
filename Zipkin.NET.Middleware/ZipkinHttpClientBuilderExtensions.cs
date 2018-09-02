using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Middleware
{
    public static class ZipkinHttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddZipkinMessageHandler(
            this IHttpClientBuilder builder, string applicationName = null)
        {
            builder.AddHttpMessageHandler(provider =>
            {
                if (applicationName == null)
                    applicationName = builder.Name;

                var reporter = provider.GetService<IReporter>();
                var traceContextAccessor = provider.GetService<ITraceContextAccessor>();
                var propagator = provider.GetService<IB3Propagator>();
                var zipkinHandler = new ZipkinHandler(
                    applicationName, reporter, traceContextAccessor, propagator);
                return zipkinHandler;
            });

            return builder;
        }
    }
}
