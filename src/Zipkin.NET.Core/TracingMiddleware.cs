using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Core.Propagation;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// .NET Core middleware used to build and report server spans from incoming requests.
    /// </summary>
    public class TracingMiddleware : IMiddleware
    {
        private readonly string _localEndpointName;
        private readonly IExtractor<HttpRequest> _extractor;

        /// <summary>
        /// Construct a new <see cref="TracingMiddleware"/>.
        /// </summary>
        /// <param name="localEndpointName">
        /// The endpoint name describes the host recording the span.
        /// </param>
        public TracingMiddleware(string localEndpointName)
        {
            _localEndpointName = localEndpointName;
            _extractor = new HttpRequestExtractor();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var trace = new ServerTrace<HttpRequest>(
                _localEndpointName, _extractor, context.Request);

            await trace.Start(_localEndpointName, async spanBuilder =>
            {
                spanBuilder
                    .Name(context.Request.Method)
                    .Tag("host", context.Request.Host.Value)
                    .Tag("resource", context.Request.Path.Value)
                    .Tag("method", context.Request.Method);

                await next(context);
            });
        }
    }
}
