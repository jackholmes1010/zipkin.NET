using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Instrumentation
{
    /// <inheritdoc />
    public class ClientTrace : Trace
    {
        public ClientTrace(TraceContext traceContext, string name, Endpoint localEndpoint = null, Endpoint remoteEndpoint = null)
            : base(traceContext, name, localEndpoint, remoteEndpoint)
        {
            Span.Kind = SpanKind.Client;
        }
    }
}
