using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// Convenience wrapper used to manage a <see cref="SpanKind.Client"/> <see cref="Span"/>.
    /// </summary>
    public class ClientTrace : Trace
    {
        /// <summary>
        /// Create a new client trace using the <see cref="TraceContext"/> of the server parent trace.
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
        public ClientTrace(TraceContext context, string name, Endpoint local = null, Endpoint remote = null)
            : base(context, name, local, remote)
        {
            Span.Kind = SpanKind.Client;
        }
    }
}
