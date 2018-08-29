using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;
using Span = Zipkin.NET.Instrumentation.Models.Span;

namespace Zipkin.NET.Middleware
{
    /// <summary>
    /// Middleware responsible for extracting trace ID's from X-B3
    /// headers and reporting completed server spans to a Zipkin server.
    /// </summary>
    public class ZipkinMiddleware : IMiddleware
    {
        private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ITraceContext _traceContext;
        private readonly ITraceIdentifierGenerator _traceIdGenerator;

        public ZipkinMiddleware(
            string applicationName,
            IReporter reporter,
            ITraceContext traceContext,
            ITraceIdentifierGenerator traceIdGenerator)
        {
            _applicationName = applicationName;
            _reporter = reporter;
            _traceContext = traceContext;
            _traceIdGenerator = traceIdGenerator;
        }

        /// <summary>
        /// Creates a new span before calling the next middleware in the
        /// pipeline. Records the duration and reports the completed span.
        /// </summary>
        /// <remarks>
        /// Completed spans contain both the server receive and server send times.
        /// </remarks>
        /// <param name="context">
        /// The <see cref="HttpContext"/> for the current request.
        /// </param>
        /// <param name="next">
        /// The delegate representing the remaining middleware in the request pipeline.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the execution of this middleware.
        /// </returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Record the server start time (span timestamp)
            var startTime = DateTime.Now;

            // Extract X-B3 headers
            var b3TraceId = context.Request.Headers.TryGetValue("X-B3-TraceId", out var value)
                ? value.ToString()
                : null;

            var b3SpanId = context.Request.Headers.TryGetValue("X-B3-SpanId", out value)
                ? value.ToString()
                : null;

            var traceId = b3TraceId ?? _traceIdGenerator.GenerateId();
            var spanId = _traceIdGenerator.GenerateId();
            var parentId = b3SpanId;

            // Record the current trace and span ID's on a shared trace
            // context so we can later retrieve the values for the client trace.
            _traceContext.CurrentTraceId = traceId;
            _traceContext.CurrentSpanId = spanId;

            var span = new Span
            {
                Id = spanId,
                TraceId = traceId,
                ParentId = parentId,
                TimeStamp = startTime,
                Name = context.Request.Method,
                Kind = SpanKind.Server,
                LocalEndpoint = new Endpoint
                {
                    ServiceName = _applicationName,
                }
            };

            // Call the next delegate/middleware in the pipeline
            await next(context);

            // Get the server send time (span duration)
            span.Duration = DateTime.Now.Subtract(startTime);

            // Report the complete span
            _reporter.Report(span);
        }
    }
}
