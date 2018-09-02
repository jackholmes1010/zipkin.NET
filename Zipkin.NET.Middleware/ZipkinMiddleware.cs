﻿using System;
using System.Diagnostics;
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
	    private readonly IB3Propagator _propagator;
		private readonly ITraceContextAccessor _traceContextAccessor;

        public ZipkinMiddleware(
            string applicationName,
            IReporter reporter,
            IB3Propagator propagator, 
            ITraceContextAccessor traceContextAccessor)
        {
            _applicationName = applicationName;
            _reporter = reporter;
	        _propagator = propagator;
	        _traceContextAccessor = traceContextAccessor;
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
            // Extract X-B3 headers
	        var serverTrace = _propagator
		        .Extract(context)
		        .Refresh();

            // Record the server trace context so we can
            // later retrieve the values for the client trace.
	        _traceContextAccessor.Context = serverTrace;

            var span = new Span(serverTrace)
            {
                Name = context.Request.Method,
                Kind = SpanKind.Server,
                LocalEndpoint = new Endpoint
                {
                    ServiceName = _applicationName,
                }
            };

            await InvokeAsync(context, next, span);
        }

        private async Task InvokeAsync(HttpContext context, RequestDelegate next, Span span)
        {
            span.RecordStartTime();

            try
            {
                // Call the next delegate/middleware in the pipeline
                await next(context);
            }
            catch (Exception ex)
            {
                // Add annotation?
                throw ex;
            }
            finally
            {
                span.RecordDuration();
                _reporter.Report(span);
            }
        }
    }
}
