using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.Instrumentation.Models;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;
using Span = Zipkin.NET.Instrumentation.Models.Span;

namespace Zipkin.NET.Middleware
{
    public class ZipkinMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IReporter _reporter;
        private readonly ITraceContext _traceContext;
        private readonly ITraceIdentifierGenerator _traceIdGenerator;

        public ZipkinMiddleware(
            RequestDelegate next, 
            IReporter reporter,
            ITraceContext traceContext,
            ITraceIdentifierGenerator traceIdGenerator)
        {
            _next = next;
            _reporter = reporter;
            _traceContext = traceContext;
            _traceIdGenerator = traceIdGenerator;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Record the server start time (span timestamp)
            var startTime = GetTimeStamp();

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
                SpanId = spanId,
                TraceId = traceId,
                ParentId = parentId,
                TimeStamp = startTime,
                Kind = SpanKind.Server
            };

            // Call the next delegate/middleware in the pipeline
            await _next(context);

            // Get the server send time (span duration)
            span.Duration = GetTimeStamp() - startTime;

            // Report the complete span
            await _reporter.ReportAsync(span);
        }

        /// <summary>
        /// Get the current timestamp.
        /// </summary>
        /// <returns>
        /// The timestamp in microseconds.
        /// </returns>
        private long GetTimeStamp()
        {
            // TODO put in helper class?
            return DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
        }
    }
}
