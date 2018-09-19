using Microsoft.Owin;
using Zipkin.NET.Constants;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.OWIN
{
    public class OwinContextB3Propagator : IPropagator<IOwinContext>
    {
        public IOwinContext Inject(IOwinContext inject, Span span)
        {
            inject.Request.Headers.Add(B3HeaderConstants.TraceId, new[] { span.TraceId });
            inject.Request.Headers.Add(B3HeaderConstants.SpanId, new[] { span.Id });
            inject.Request.Headers.Add(B3HeaderConstants.ParentSpanId, new[] { span.ParentId });
            //inject.Request.Headers.Add(B3HeaderConstants.Sampled, new[] { span.Sampled == true ? "1" : "0" });
            inject.Request.Headers.Add(B3HeaderConstants.Flags, new[] { span.Debug ? "1" : "0" });
            return inject;
        }
    }
}
