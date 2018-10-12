using System.Net.Http;
using Zipkin.NET.Constants;

namespace Zipkin.NET.Propagation
{
    /// <summary>
    /// Propagates span context by adding X-B3 headers to a <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class HttpRequestMessagePropagator : Propagator<HttpRequestMessage>
    {
        /// <summary>
        /// Propagate span context by adding X-B3 headers to the <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="inject">
        /// The <see cref="HttpRequestMessage"/>.
        /// </param>
        /// <param name="spanContext">
        /// The span context which contains trace ID, sampling and debug info.
        /// </param>
        /// <returns>
        /// The <see cref="HttpRequestMessage"/> with X-B3 headers.
        /// </returns>
        protected override HttpRequestMessage Inject(HttpRequestMessage inject, SpanContext spanContext)
        {
            inject.Headers.Add(B3HeaderConstants.TraceId, spanContext.TraceId);
            inject.Headers.Add(B3HeaderConstants.SpanId, spanContext.Id);
            inject.Headers.Add(B3HeaderConstants.ParentSpanId, spanContext.ParentId);
            inject.Headers.Add(B3HeaderConstants.Sampled, spanContext.Sampled == true ? "1" : "0");
            inject.Headers.Add(B3HeaderConstants.Flags, spanContext.Debug ? "1" : "0");
            return inject;
        }
    }
}
