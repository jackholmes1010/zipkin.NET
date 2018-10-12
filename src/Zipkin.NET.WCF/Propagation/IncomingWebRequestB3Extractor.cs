using System.ServiceModel.Web;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.WCF.Propagation
{
    public class IncomingWebRequestB3Extractor : IExtractor<IncomingWebRequestContext>
    {
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
