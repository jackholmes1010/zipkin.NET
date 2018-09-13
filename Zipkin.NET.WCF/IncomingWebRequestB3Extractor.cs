using System.ServiceModel.Web;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Propagation;

namespace Zipkin.NET.WCF
{
    /// <summary>
    /// Extracts a <see cref="TraceContext"/> from a <see cref="IncomingWebRequestContext"/>.
    /// </summary>
    public class IncomingWebRequestB3Extractor : IExtractor<IncomingWebRequestContext>
    {
        /// <summary>
        /// Extracts the X-B3 trace ID header values from the request.
        /// </summary>
        /// <param name="context">
        /// The incoming request.
        /// </param>
        /// <returns>
        /// A <see cref="TraceContext"/> containing the header values.
        /// </returns>
        public TraceContext Extract(IncomingWebRequestContext context)
        {
            var traceContext = new TraceContext
            {
                TraceId = context.Headers[B3HeaderConstants.TraceId],
                SpanId = context.Headers[B3HeaderConstants.SpanId],
                Debug = context.Headers[B3HeaderConstants.Flags] == "1"
            };

            if (context.Headers[B3HeaderConstants.Sampled] != null)
                traceContext.Sampled = true;

            return traceContext;
        }
    }
}
