using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Core
{
    public static class ZipkinServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(
            this IServiceCollection services, string applicationName, string zipkinHost)
        {
            // TODO is this needed?
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISampler, DebugSampler>();
            services.AddTransient<IReporter, Reporter>();
            services.AddTransient<ISender>(provider => new HttpSender(zipkinHost));
            services.AddTransient<ITraceContextAccessor, HttpContextTraceContextAccessor>();
            services.AddTransient<IPropagator<HttpRequest, HttpRequestMessage>, B3Propagator>();

            // Register middleware
            services.AddTransient(provider =>
            {
                var reporter = provider.GetService<IReporter>();
                var propagator = provider.GetService<IPropagator<HttpRequest, HttpRequestMessage>>();
                var traceContextAccessor = provider.GetService<ITraceContextAccessor>();
                var middleware = new ZipkinMiddleware(
                    applicationName, reporter, traceContextAccessor, propagator);
                return middleware;
            });

            return services;
        }
    }
}
