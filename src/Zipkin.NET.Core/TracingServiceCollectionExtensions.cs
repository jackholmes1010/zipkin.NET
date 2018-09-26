using Microsoft.Extensions.DependencyInjection;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class TracingServiceCollectionExtensions
    {
        /// <summary>
        /// Add the <see cref="TracingMiddleware"/> to the service collection.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="localEndpointName">
        /// The endpoint name describes the host recording the span.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddTracingMiddleware(this IServiceCollection services, string localEndpointName)
        {
            services.AddTransient(provider => new TracingMiddleware(localEndpointName));
            return services;
        }
    }
}
