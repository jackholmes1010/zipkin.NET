using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// Middleware responsible for extracting trace ID's from X-B3
    /// headers and reporting completed server spans to a Zipkin server.
    /// </summary>
    public class ZipkinMiddleware : IMiddleware
    {
        private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ITraceContextAccessor _traceContextAccessor;
	    private readonly IPropagator<HttpRequest, HttpRequestMessage> _propagator;

		public ZipkinMiddleware(
            string applicationName,
            IReporter reporter,
            ITraceContextAccessor traceContextAccessor,
			IPropagator<HttpRequest, HttpRequestMessage> propagator)
        {
            _applicationName = applicationName;
            _reporter = reporter;
            _traceContextAccessor = traceContextAccessor;
	        _propagator = propagator;
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
            var traceContext = _propagator
                .Extract(context.Request)
                .NewChild();

            // Record the server trace context so we can
            // later retrieve the values for the client trace.
            _traceContextAccessor.Context = traceContext;

            var serverTrace = new ServerTrace(
                traceContext, 
                context.Request.Method, 
                localEndpoint: new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Record server recieve start time and start duration timer
            serverTrace.Start();

            try
            {
                await next(context);

            }
            catch (Exception ex)
            {
                serverTrace.Error(ex.Message);
                throw;
            }
            finally
            {
                serverTrace.End();

                if (traceContext.Sample())
	                // Report completed span to Zipkin
	                _reporter.Report(serverTrace.Span);
			}
        }
    }
}
