using System.Net.Http;
using Zipkin.NET.Instrumentation.Constants;

namespace Zipkin.NET.Instrumentation.Propagation
{
    /// <summary>
    /// Injects trace context into an <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class HttpRequestMessageB3Propagator : IPropagator<HttpRequestMessage>
    {
        /// <summary>
        /// Adds X-B3 header values to an outgoing HTTP request.
        /// </summary>
        /// <param name="request">
        /// The request to add headers to.
        /// </param>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/> which contains trace ID context for the current trace.
        /// </param>
        /// <returns>
        /// The HTTP request with trace headers.
        /// </returns>
        public HttpRequestMessage Inject(HttpRequestMessage request, TraceContext traceContext)
        {
            request.Headers.Add(B3HeaderConstants.TraceId, traceContext.TraceId);
            request.Headers.Add(B3HeaderConstants.SpanId, traceContext.SpanId);
            request.Headers.Add(B3HeaderConstants.ParentSpanId, traceContext.ParentSpanId);
            request.Headers.Add(B3HeaderConstants.Sampled, traceContext.Sampled == true ? "1" : "0");
            request.Headers.Add(B3HeaderConstants.Flags, traceContext.Debug == true ? "1" : "0");
            return request;
        }
    }
}
