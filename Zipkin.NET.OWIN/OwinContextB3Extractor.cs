using System.Linq;
using Microsoft.Owin;
using Zipkin.NET.Constants;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.OWIN
{
    public class OwinContextB3Extractor : IExtractor<IOwinContext>
    {
        public Span Extract(IOwinContext extract)
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

            //bool? sampled = null;
            //if (extract.Request.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
            //{
            //    sampled = value.FirstOrDefault() == "1";
            //}

            bool? debug = null;
            if (extract.Request.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                debug = value.FirstOrDefault() == "1";
            }

            return new Span
            {
                TraceId = traceId,
                Id = spanId,
                Debug = debug == true
            };
        }
    }
}
