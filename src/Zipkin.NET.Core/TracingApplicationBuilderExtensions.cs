using System;
using Microsoft.AspNetCore.Builder;

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

            return app.UseMiddleware<TracingMiddleware>();
        }
    }
}
