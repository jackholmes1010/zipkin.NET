using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;

namespace Zipkin.NET.Middleware
{
    /// <summary>
    /// Extracts and adds X-B3 header values to HTTP requests.
    /// </summary>
    public class B3Propagator : IB3Propagator
    {
        /// <summary>
        /// Extracts the X-B3 trace ID header values from the request.
        /// </summary>
        /// <param name="httpContext">
        /// The <see cref="HttpContext"/> which contains the request.
        /// </param>
        /// <returns>
        /// A <see cref="TraceContext"/> containing the header values.
        /// </returns>
        public TraceContext Extract(HttpContext httpContext)
        {
            var traceContext = new TraceContext();

            if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                traceContext.TraceId = value;
            }

            if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                traceContext.SpanId = value;
            }

	        if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
	        {
		        traceContext.Sampled = value == "1";
	        }

	        if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
	        {
		        traceContext.Debug = value == "1";
	        }

            return traceContext;
        }

        /// <summary>
        /// Adds X-B3 header values to an outgoing HTTP request.
        /// </summary>
        /// <param name="request">
        /// The request to add headers to.
        /// </param>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/> which contains trace ID context for the current trace.
        /// </param>
        /// <returns></returns>
        public HttpRequestMessage Inject(HttpRequestMessage request, TraceContext traceContext)
        {
            request.Headers.Add(B3HeaderConstants.TraceId, traceContext.TraceId);
            request.Headers.Add(B3HeaderConstants.SpanId, traceContext.SpanId);
            request.Headers.Add(B3HeaderConstants.ParentSpanId, traceContext.ParentSpanId);
            request.Headers.Add(B3HeaderConstants.Sampled, traceContext.Sampled ? "1" : "0");
            request.Headers.Add(B3HeaderConstants.Flags, traceContext.Debug ? "1" : "0");
            return request;
        }
    }
}
