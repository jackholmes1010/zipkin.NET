using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// Convenience wrapper used to manage a <see cref="SpanKind.Server"/> <see cref="Span"/>.
    /// </summary>
    public class ServerTrace : Trace
    {
        /// <summary>
        /// Create a new server trace using the <see cref="TraceContext"/> of the client parent trace.
        /// </summary>
        /// <param name="context">
        /// The parent's <see cref="TraceContext"/>.
        /// </param>
        /// <param name="name">
        /// The logical name of this operation.
        /// </param>
        /// <param name="local">
        /// The local network context.
        /// </param>
        /// <param name="remote">
        /// The remote network context.
        /// </param>
        public ServerTrace(TraceContext traceContext, string name, Endpoint localEndpoint = null, Endpoint remoteEndpoint = null)
            : base(traceContext, name, localEndpoint, remoteEndpoint)
        {
            Span.Kind = SpanKind.Server;
        }
    }
}
