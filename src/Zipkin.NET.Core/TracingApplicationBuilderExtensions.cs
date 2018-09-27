using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zipkin.NET.Core.Logging;
using Zipkin.NET.Core.TraceAccessors;
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
        /// <param name="reporters">
        /// A collection of <see cref="IReporter"/>s used by the <see cref="Tracer"/> to report completed spans.
        /// </param>
        /// <param name="sampler">
        /// A <see cref="Sampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context in the context of the current request.
        /// </param>
        /// <param name="instrumentationLogger">
        /// A <see cref="IInstrumentationLogger"/> used by instrumentation to log errors.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationBuilder"/>.
        /// </returns>
        public static IApplicationBuilder UseTracer(
            this IApplicationBuilder app,
            IEnumerable<IReporter> reporters,
            Sampler sampler,
            ITraceContextAccessor traceContextAccessor = null,
            IInstrumentationLogger instrumentationLogger = null)
        {
            if (traceContextAccessor == null)
            {
                var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();
                traceContextAccessor = new HttpContextTraceContextAccessor(httpContextAccessor);
            }

            if (instrumentationLogger == null)
            {
                var logger = app.ApplicationServices.GetService<ILogger<CoreInstrumentationLogger>>();
                instrumentationLogger = new CoreInstrumentationLogger(logger);
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
