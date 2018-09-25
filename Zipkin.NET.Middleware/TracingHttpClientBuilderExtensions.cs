using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Propagation;

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

                var traceAccessor = provider.GetService<ITraceAccessor>();
                var propagator = provider.GetService<IPropagator<HttpRequestMessage>>();

                var handler = innerHandler != null
                    ? new TracingHandler(innerHandler, applicationName, traceAccessor, propagator)
                    : new TracingHandler(applicationName, traceAccessor, propagator);

                return handler;
            });

            return builder;
        }
    }
}
