using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Instrumentation
{
    /// <inheritdoc />
    public class ServerTrace : Trace
    {
        public ServerTrace(TraceContext traceContext, string name, Endpoint localEndpoint = null, Endpoint remoteEndpoint = null)
            : base(traceContext, name, localEndpoint, remoteEndpoint)
        {
            Span.Kind = SpanKind.Server;
        }
    }
}
