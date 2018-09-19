using System.ServiceModel.Web;
using Zipkin.NET.Constants;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.WCF
{
    public class IncomingWebRequestB3Extractor : IExtractor<IncomingWebRequestContext>
    {
        public Span Extract(IncomingWebRequestContext extract)
        {
            var span = new Span
            {
                TraceId = extract.Headers[B3HeaderConstants.TraceId],
                Id = extract.Headers[B3HeaderConstants.SpanId],
                Debug = extract.Headers[B3HeaderConstants.Flags] == "1"
            };

            //if (context.Headers[B3HeaderConstants.Sampled] != null)
            //    traceContext.Sampled = true;

            return span;
        }
    }
}
