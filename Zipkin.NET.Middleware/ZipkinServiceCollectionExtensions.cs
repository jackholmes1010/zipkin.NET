using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Middleware
{
    public static class ZipkinServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(this IServiceCollection services, string applicationName)
        {
            // TODO is this needed?
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITraceIdentifierGenerator, TraceIdentifierGenerator>();
            services.AddTransient<IReporter, Reporter>();
            services.AddTransient<ISender, HttpSender>();
            services.AddTransient<ITraceContextAccessor, TraceContextAccessor>();
            services.AddTransient<IB3Propagator, B3Propagator>();

            // Register middleware
            services.AddTransient(provider =>
            {
                var reporter = provider.GetService<IReporter>();
                var propagator = provider.GetService<IB3Propagator>();
                var traceContextAccessor = provider.GetService<ITraceContextAccessor>();
                var middleware = new ZipkinMiddleware(
                    applicationName, reporter, propagator, traceContextAccessor);
                return middleware;
            });

            return services;
        }
    }
}
