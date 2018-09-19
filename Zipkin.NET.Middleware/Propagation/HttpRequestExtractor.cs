using Microsoft.AspNetCore.Http;
using Zipkin.NET.Constants;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Middleware.Propagation
{
    public class HttpRequestExtractor : IExtractor<HttpRequest>
    {
        public Trace Extract(HttpRequest extract)
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
            

            return new Trace(traceId, spanId)
            {
                Debug = debug,
                Sampled = sampled
            };
        }
    }
}
