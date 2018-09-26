using System.Linq;
using Microsoft.Owin;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.OWIN
{
    /// <summary>
    /// Used to extract a <see cref="TraceContext"/> from a 
    /// <see cref="IOwinRequest"/> inside an <see cref="IOwinContext"/>.
    /// </summary>
    public class OwinContextB3Extractor : IExtractor<IOwinContext>
    {
        /// <summary>
        /// Extracts a <see cref="TraceContext"/> from an <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="extract">
        /// The <see cref="IOwinContext"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext Extract(IOwinContext extract)
        {
            string traceId = null;
            if (extract.Request.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                traceId = value.FirstOrDefault();
            }

            string spanId = null;
            if (extract.Request.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                spanId = value.FirstOrDefault();
            }

            var debug = false;
            if (extract.Request.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                debug = value.FirstOrDefault() == "1";
            }

            bool? sampled = null;
            if (extract.Request.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
            {
                sampled = value.FirstOrDefault() == "1";
            }

            return new TraceContext(traceId, spanId)
            {
                Debug = debug,
                Sampled = sampled
            };
        }
    }
}
