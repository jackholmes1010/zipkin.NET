using System.Runtime.Remoting.Messaging;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the current <see cref="TraceContext"/> in the logical <see cref="CallContext"/>.
    /// <remarks>
    /// This will not work if an application is hosted using IIS since the 
    /// middleware's context is not shared with the controller call context.
    /// </remarks>
    /// </summary>
    public class CallContextTraceContextAccessor : ITraceContextAccessor
    {
        private const string ContextKey = "server-trace";

        /// <summary>
        /// Stores a <see cref="TraceContext"/> on the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// </param>
        public void SaveTrace(TraceContext traceContext)
        {
            CallContext.LogicalSetData(ContextKey, traceContext);
        }

        /// <summary>
        /// Retrieves a <see cref="TraceContext"/> from the <see cref="CallContext"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext GetTrace()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace as TraceContext;
        }

        /// <summary>
        /// Checks if a <see cref="TraceContext"/> is stored on the <see cref="CallContext"/>.
        /// </summary>
        /// <returns>
        /// True if a trace context is stored, else false.
        /// </returns>
        public bool HasTrace()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace is TraceContext;
        }
    }
}
