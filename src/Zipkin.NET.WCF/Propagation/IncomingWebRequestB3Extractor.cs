using System.ServiceModel.Web;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.WCF.Propagation
{
    public class IncomingWebRequestB3Extractor : IExtractor<IncomingWebRequestContext>
    {
        public TraceContext Extract(IncomingWebRequestContext extract)
        {
            var traceId = extract.Headers[B3HeaderConstants.TraceId];
            var spanId = extract.Headers[B3HeaderConstants.SpanId];
            var debug = extract.Headers[B3HeaderConstants.Flags] == "1";

            bool? sampled = null;
            if (extract.Headers[B3HeaderConstants.Sampled] != null)
                sampled = true;

            return new TraceContext(traceId, spanId)
            {
                Debug = debug,
                Sampled = sampled
            };
        }
    }
}
