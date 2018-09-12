using System.Linq;
using Microsoft.Owin;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.OWIN
{
    /// <summary>
    /// Extracts a <see cref="TraceContext"/> from a <see cref="OwinContext"/>.
    /// </summary>
    public class B3Extractor : IExtractor<IOwinContext>
    {
        /// <summary>
        /// Extracts the X-B3 trace ID header values from the request.
        /// </summary>
        /// <param name="context">
        /// The current OWIN context.
        /// </param>
        /// <returns>
        /// A <see cref="TraceContext"/> containing the header values.
        /// </returns>
        public TraceContext Extract(IOwinContext context)
        {
            string traceId = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                traceId = value.FirstOrDefault();
            }

            string spanId = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                spanId = value.FirstOrDefault();
            }

            bool? sampled = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
            {
                sampled = value.FirstOrDefault() == "1";
            }

            bool? debug = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                debug = value.FirstOrDefault() == "1";
            }

            return new TraceContext
            {
                TraceId = traceId,
                SpanId = spanId,
                Sampled = sampled,
                Debug = debug
            };
        }
    }
}
