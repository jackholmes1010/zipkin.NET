using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Middleware
{
    /// <summary>
    /// Extracts and adds X-B3 header values to HTTP requests.
    /// </summary>
    public class B3Propagator : IB3Propagator
    {
	    private readonly ITraceIdentifierGenerator _traceIdGenerator;
	    private readonly ISampler _sampler;

	    public B3Propagator(ITraceIdentifierGenerator traceIdGenerator, ISampler sampler)
	    {
		    _traceIdGenerator = traceIdGenerator;
		    _sampler = sampler;
	    }

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
		    string traceId = null;
			if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                traceId = value;
            }

		    string spanId = null;
			if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                spanId = value;
            }

		    bool? sampled = null;
			if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
            {
                sampled = value == "1";
            }

		    bool? debug = null;
			if (httpContext.Request.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                debug = value == "1";
            }

            return new TraceContext(_traceIdGenerator, _sampler)
            {
				TraceId = traceId,
				SpanId = spanId,
				Sampled = sampled,
				Debug = debug
            };
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
            request.Headers.Add(B3HeaderConstants.Sampled, traceContext.Sampled ==true ? "1" : "0");
            request.Headers.Add(B3HeaderConstants.Flags, traceContext.Debug == true ? "1" : "0");
            return request;
        }
    }
}
