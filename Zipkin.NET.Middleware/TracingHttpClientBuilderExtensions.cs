using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

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

                var handler = innerHandler != null
                    ? new TracingHandler(innerHandler, applicationName)
                    : new TracingHandler(applicationName);

                return handler;
            });

            return builder;
        }
    }
}
