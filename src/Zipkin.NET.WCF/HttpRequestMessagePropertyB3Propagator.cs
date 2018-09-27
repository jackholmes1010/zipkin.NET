using System.ServiceModel.Channels;
using Zipkin.NET.Constants;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.WCF
{
    /// <summary>
    /// Propagates trace context by adding X-B3 headers to a <see cref="HttpRequestMessageProperty"/>.
    /// </summary>
    public class HttpRequestMessagePropertyB3Propagator : IPropagator<HttpRequestMessageProperty>
    {
        /// <summary>
        /// Propagate trace context by adding X-B3 headers to the <see cref="HttpRequestMessageProperty"/>.
        /// </summary>
        /// <param name="inject">
        /// The <see cref="HttpRequestMessageProperty"/>.
        /// </param>
        /// <param name="span">
        /// A <see cref="Span"/>.
        /// </param>
        /// <param name="sampled">
        /// True if the trace is sampled, else false.
        /// </param>
        /// <returns>
        /// The <see cref="HttpRequestMessageProperty"/> with X-B3 headers.
        /// </returns>
        public HttpRequestMessageProperty Inject(HttpRequestMessageProperty inject, Span span, bool sampled)
        {
            inject.Headers.Add(B3HeaderConstants.TraceId, span.TraceId);
            inject.Headers.Add(B3HeaderConstants.SpanId, span.Id);
            inject.Headers.Add(B3HeaderConstants.ParentSpanId, span.ParentId);
            inject.Headers.Add(B3HeaderConstants.Sampled, sampled ? "1" : "0");
            inject.Headers.Add(B3HeaderConstants.Flags, span.Debug ? "1" : "0");
            return inject;
        }
    }
}
