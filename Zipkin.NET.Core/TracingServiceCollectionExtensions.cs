using Microsoft.Extensions.DependencyInjection;

namespace Zipkin.NET.Core
{
    public static class TracingServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(this IServiceCollection services, string applicationName)
        {
            services.AddTransient(provider => new TracingMiddleware(applicationName));
            return services;
        }
    }
}
