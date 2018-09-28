using System.Net.Http;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET
{
    /// <summary>
    /// Propagates trace context by adding X-B3 headers to a <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class HttpRequestMessagePropagator : Propagator<HttpRequestMessage>
    {
        /// <summary>
        /// Propagate trace context by adding X-B3 headers to the <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="inject">
        /// The <see cref="HttpRequestMessage"/>.
        /// </param>
        /// <param name="traceContext">
        /// The trace context which contains trace ID, sampling and debug info.
        /// </param>
        /// <returns>
        /// The <see cref="HttpRequestMessage"/> with X-B3 headers.
        /// </returns>
        protected override HttpRequestMessage Inject(HttpRequestMessage inject, TraceContext traceContext)
        {
            inject.Headers.Add(B3HeaderConstants.TraceId, traceContext.TraceId);
            inject.Headers.Add(B3HeaderConstants.SpanId, traceContext.Id);
            inject.Headers.Add(B3HeaderConstants.ParentSpanId, traceContext.ParentId);
            inject.Headers.Add(B3HeaderConstants.Sampled, traceContext.Sampled == true ? "1" : "0");
            inject.Headers.Add(B3HeaderConstants.Flags, traceContext.Debug ? "1" : "0");
            return inject;
        }
    }
}
