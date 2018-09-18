using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zipkin.NET.Middleware.Propagation;
using Zipkin.NET.Middleware.TraceAccessors;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;
using Zipkin.NET.Senders;

namespace Zipkin.NET.Middleware
{
    public static class TracingServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(
            this IServiceCollection services, string applicationName, string zipkinHost)
        {
            // Register default services
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddTransient<IReporter, Reporter>();
            services.TryAddTransient<ISender>(provider => new HttpSender(zipkinHost));
            services.TryAddTransient<ITraceAccessor, HttpContextTraceAccessor>();
            services.TryAddTransient<IExtractor<HttpRequest>, HttpRequestExtractor>();
            services.TryAddTransient<IPropagator<HttpRequestMessage>, HttpRequestMessagePropagator>();

            services.AddTransient(provider =>
            {
                var extractor = provider.GetService<IExtractor<HttpRequest>>();
                var traceAccessor = provider.GetService<ITraceAccessor>();
                var reporter = provider.GetService<IReporter>();
                return new TracingMiddleware(applicationName, extractor, traceAccessor, reporter);
            });

            return services;
        }
    }
}
