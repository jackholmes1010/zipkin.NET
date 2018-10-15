using System.ServiceModel.Web;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.WCF.Propagation
{
    /// <summary>
    /// Extracts <see cref="SpanContext"/> from a <see cref="IncomingWebRequestContext"/>.
    /// </summary>
    public class IncomingWebRequestB3SpanContextExtractor : ISpanContextExtractor<IncomingWebRequestContext>
    {
        /// <summary>
        /// Extract <see cref="SpanContext"/> from a <see cref="IncomingWebRequestContext"/>
        /// by reading the values of the X-B3 HTTP headers.
        /// </summary>
        /// <param name="extract">
        /// The object from which to extract the span context.
        /// </param>
        /// <returns>
        /// The <see cref="SpanContext"/>.
        /// </returns>
        public SpanContext Extract(IncomingWebRequestContext extract)
        {
            var traceId = extract.Headers[B3HeaderConstants.TraceId];
            var spanId = extract.Headers[B3HeaderConstants.SpanId];
            var debug = extract.Headers[B3HeaderConstants.Flags] == "1";

            bool? sampled = null;
            var sampledHeader = extract.Headers[B3HeaderConstants.Sampled];
            if (sampledHeader != null)
            {
                sampled = sampledHeader == "1";
            }

            return new SpanContext(traceId, spanId)
            {
                Debug = debug,
                Sampled = sampled
            };
        }
    }
}
