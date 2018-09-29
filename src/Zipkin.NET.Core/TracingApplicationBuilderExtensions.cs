using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Dispatchers;
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
        public static IApplicationBuilder UseTracing(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            Tracer.Start(
                app.ApplicationServices.GetService<Sampler>(),
                app.ApplicationServices.GetService<Dispatcher>(),
                app.ApplicationServices.GetService<ITraceContextAccessor>(),
                app.ApplicationServices.GetService<IInstrumentationLogger>());

            return app.UseMiddleware<TracingMiddleware>();
        }
    }
}
