using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zipkin.NET.Core.Logging;
using Zipkin.NET.Core.TraceAccessors;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Logging;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add default tracing dependencies to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="localEndpointName"></param>
        /// <returns></returns>
        public static IServiceCollection AddTracing(this IServiceCollection services, string localEndpointName)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register async dispatcher.
            // This dispatcher will asynchronously report spans to the registered reporters.
            services.TryAddSingleton<Dispatcher, AsyncActionBlockDispatcher>();

            // Register rate sampler.
            // This RateSampler will sample 100% of traces providing a
            // sampling decision has not already been made by an upstream service.
            services.TryAddSingleton<Sampler>(provider => new RateSampler(1f));

            // Register the trace context accessor.
            // This ITraceContextAccessor will store the trace context in the HTTP context.
            // This allows middleware to store trace context (trace ID, server span ID, debug flag, and sampled 
            // flag) for use by the tracing handler (HTTP client delegating handler) for creating client spans.
            services.TryAddTransient<ITraceContextAccessor, HttpContextTraceContextAccessor>();

            // Register .NET Core ILogger tracing logger (used for exception logging).
            // This logger logs instrumentation exceptions using the .NET Core ILogger.
            services.TryAddSingleton<IInstrumentationLogger, CoreInstrumentationLogger>();

            // Register tracing middleware.
            // This middleware builds spans from incoming requests 
            // and reports them to the registered IReporters.
            services.TryAddTransient(s => new TracingMiddleware(
                localEndpointName,
                s.GetService<ITraceContextAccessor>(),
                s.GetService<Dispatcher>(),
                s.GetService<Sampler>()));

            return services;
        }
    }
}
