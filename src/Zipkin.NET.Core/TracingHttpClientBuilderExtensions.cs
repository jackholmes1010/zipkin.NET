using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// <see cref="IHttpClientBuilder"/> extension methods.
    /// </summary>
    public static class TracingHttpClientBuilderExtensions
    {
        /// <summary>
        /// Adds a delegating handler used to report client spans and propagate trace context.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IHttpClientBuilder"/>.
        /// </param>
        /// <param name="remoteEndpointName">
        /// The remote endpoint name describes the reciever.
        /// </param>
        /// <param name="innerHandler">
        /// An inner handler.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpClientBuilder"/>.
        /// </returns>
        public static IHttpClientBuilder AddTracingMessageHandler(
            this IHttpClientBuilder builder, string remoteEndpointName = null, HttpMessageHandler innerHandler = null)
        {
            builder.AddHttpMessageHandler(provider =>
            {
                if (remoteEndpointName == null)
                    remoteEndpointName = builder.Name;

                var handler = innerHandler != null
                    ? new TracingHandler(
                        innerHandler,
                        provider.GetService<ITraceContextAccessor>(),
                        provider.GetService<Dispatcher>(),
                        provider.GetService<Sampler>(),
                        remoteEndpointName)
                    : new TracingHandler(
                        provider.GetService<ITraceContextAccessor>(),
                        provider.GetService<Dispatcher>(),
                        provider.GetService<Sampler>(),
                        remoteEndpointName);

                return handler;
            });

            return builder;
        }
    }
}
