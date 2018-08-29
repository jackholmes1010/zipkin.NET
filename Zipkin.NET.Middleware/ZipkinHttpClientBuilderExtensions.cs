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
                var traceContext = provider.GetService<ITraceContext>();
                var traceIdGenerator = provider.GetService<ITraceIdentifierGenerator>();
                var zipkinHandler = new ZipkinHandler(applicationName, reporter, traceContext, traceIdGenerator);
                return zipkinHandler;
            });

            return builder;
        }
    }
}
