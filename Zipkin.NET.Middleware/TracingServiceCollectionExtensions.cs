using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zipkin.NET.Middleware.Propagation;
using Zipkin.NET.Middleware.TraceAccessors;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Middleware
{
    public static class TracingServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(this IServiceCollection services, string applicationName)
        {
            // Register default services
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddTransient<IPropagator<HttpRequestMessage>, HttpRequestMessagePropagator>();

            services.AddTransient(provider =>
            {
                var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
                return new TracingMiddleware(applicationName, httpContextAccessor);
            });

            return services;
        }
    }
}
