using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Instrumentation
{
    /// <inheritdoc />
    public class ClientTrace : Trace
    {
        public ClientTrace(ISampler sampler, TraceContext traceContext, string name, Endpoint localEndpoint = null, Endpoint remoteEndpoint = null)
            : base(sampler, traceContext, name, localEndpoint, remoteEndpoint)
        {
            Span.Kind = SpanKind.Client;
        }
    }
}
