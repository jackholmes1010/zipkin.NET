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

        public void SaveTrace(TraceContext traceContext)
        {
            CallContext.LogicalSetData(ContextKey, traceContext);
        }

        public TraceContext GetTrace()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace as TraceContext;
        }

        public bool HasTrace()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace is TraceContext;
        }
    }
}
