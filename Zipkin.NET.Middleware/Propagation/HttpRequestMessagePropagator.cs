using System.Net.Http;
using Zipkin.NET.Constants;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.Middleware.Propagation
{
    public class HttpRequestMessagePropagator : IPropagator<HttpRequestMessage>
    {
        public HttpRequestMessage Inject(HttpRequestMessage inject, Span span)
        {
            inject.Headers.Add(B3HeaderConstants.TraceId, span.TraceId);
            inject.Headers.Add(B3HeaderConstants.SpanId, span.Id);
            inject.Headers.Add(B3HeaderConstants.ParentSpanId, span.ParentId);
            //inject.Headers.Add(B3HeaderConstants.Sampled, span.Sampled == true ? "1" : "0");
            inject.Headers.Add(B3HeaderConstants.Flags, span.Debug == true ? "1" : "0");
            return inject;
        }
    }
}
