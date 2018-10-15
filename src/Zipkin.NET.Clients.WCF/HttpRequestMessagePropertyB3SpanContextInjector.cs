using System.ServiceModel.Channels;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Propagates span context by adding X-B3 headers to a <see cref="HttpRequestMessageProperty"/>.
    /// </summary>
    public class HttpRequestMessagePropertyB3SpanContextInjector : SpanContextInjector<HttpRequestMessageProperty>
    {
        /// <summary>
        /// Propagate span context by adding X-B3 headers to the <see cref="HttpRequestMessageProperty"/>.
        /// </summary>
        /// <param name="inject">
        /// The <see cref="HttpRequestMessageProperty"/>.
        /// </param>
        /// <param name="spanContext">
        /// The span context which contains trace ID, sampling and debug info.
        /// </param>
        /// <returns>
        /// The <see cref="HttpRequestMessageProperty"/> with X-B3 headers.
        /// </returns>
        protected override HttpRequestMessageProperty InjectSpanContext(HttpRequestMessageProperty inject, SpanContext spanContext)
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
