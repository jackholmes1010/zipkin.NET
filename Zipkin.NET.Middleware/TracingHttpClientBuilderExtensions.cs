using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Middleware
{
    public static class TracingHttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddZipkinMessageHandler(
            this IHttpClientBuilder builder, string applicationName = null, HttpMessageHandler innerHandler = null)
        {
            builder.AddHttpMessageHandler(provider =>
            {
                if (applicationName == null)
                    applicationName = builder.Name;

                var reporter = provider.GetService<IReporter>();
                var traceAccessor = provider.GetService<ITraceAccessor>();
                var propagator = provider.GetService<IPropagator<HttpRequestMessage>>();

                var handler = innerHandler != null
                    ? new TracingHandler(innerHandler, applicationName, reporter, traceAccessor, propagator)
                    : new TracingHandler(applicationName, reporter, traceAccessor, propagator);

                return handler;
            });

            return builder;
        }
    }
}
