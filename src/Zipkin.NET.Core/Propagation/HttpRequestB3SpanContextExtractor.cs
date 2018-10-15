using Microsoft.AspNetCore.Http;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Core.Propagation
{
    /// <summary>
    /// Extracts a <see cref="SpanContext"/> from an <see cref="HttpRequest"/>.
    /// </summary>
    public class HttpRequestB3SpanContextExtractor : ISpanContextExtractor<HttpRequest>
    {
        /// <summary>
        /// Extract a <see cref="SpanContext"/> from a 
        /// <see cref="HttpRequest"/> by reading the values of the X-B3 headers.
        /// </summary>
        /// <param name="extract">
        /// The <see cref="HttpContext" />
        /// </param>
        /// <returns>
        /// The extracted <see cref="SpanContext"/>.
        /// </returns>
        public SpanContext Extract(HttpRequest extract)
        {
            string traceId = null;
            if (extract.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                traceId = value;
            }

            string spanId = null;
            if (extract.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                spanId = value;
            }

            var debug = false;
            if (extract.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                debug = value == "1";
            }

            bool? sampled = null;
            if (extract.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
            {
                sampled = value == "1";
            }
            
            return new SpanContext(traceId, spanId)
            {
                Debug = debug,
                Sampled = sampled
            };
        }
    }
}
