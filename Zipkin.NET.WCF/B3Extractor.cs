using System.ServiceModel.Web;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.WCF
{
    public class B3Extractor : IExtractor<IncomingWebRequestContext>
    {
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
