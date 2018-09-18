using Microsoft.AspNetCore.Http;
using Zipkin.NET.Constants;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Middleware.Propagation
{
    public class HttpRequestExtractor : IExtractor<HttpRequest>
    {
        public Span Extract(HttpRequest extract)
        {
            var span = new Span();

            if (extract.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                span.TraceId = value;
            }

            if (extract.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                span.ParentId = value;
            }

            if (extract.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                span.Debug = value == "1";
            }
            else
            {
                span.Debug = false;
            }

            return span;
        }
    }
}
