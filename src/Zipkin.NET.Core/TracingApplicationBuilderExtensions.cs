using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension methods.
    /// </summary>
    public static class TracingApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="TracingMiddleware"/> to the request pipeline.
        /// </summary>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationBuilder"/>.
        /// </returns>
        public static IApplicationBuilder UseTracingMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<TracingMiddleware>();
        }

        /// <summary>
        /// Starts the static <see cref="Tracer"/> used to report completed spans.
        /// </summary>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationBuilder"/>.
        /// </returns>
        public static IApplicationBuilder UseTracer(this IApplicationBuilder app)
        {
            var sampler = app.ApplicationServices.GetService<Sampler>();

            if (sampler == null)
            {
                throw new Exception(
                    "Sampler not registered with service collection. Register a Sampler in the ConfigureServices method.");
            }

            var traceContextAccessor = app.ApplicationServices.GetService<ITraceContextAccessor>();

            if (traceContextAccessor == null)
            {
                throw new Exception(
                    "No ITraceContextAccessor registered with service collection. Register an ITraceContextAccessor in the ConfigureServices method.");
            }

            var instrumentationLogger = app.ApplicationServices.GetService<IInstrumentationLogger>();

            if (traceContextAccessor == null)
            {
                throw new Exception(
                    "No IInstrumentationLogger registered with service collection. Register an IInstrumentationLogger in the ConfigureServices method.");
            }

            var reporters = app.ApplicationServices.GetServices<IReporter>()?.ToList();

            if (reporters == null || !reporters.Any())
            {
                throw new Exception(
                    "No IReporters registered with service collection. Register an IReporter in the ConfigureServices method.");
            }

            Tracer.Start(
                sampler,
                traceContextAccessor,
                instrumentationLogger,
                reporters);

            return app;
        }
    }
}
