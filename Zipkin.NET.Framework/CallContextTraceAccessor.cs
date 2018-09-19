using System.Runtime.Remoting.Messaging;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the current <see cref="Trace"/> in the logical <see cref="CallContext"/>.
    /// <remarks>
    /// This will not work if an application is hosted using IIS since the 
    /// middleware's context is not shared with the controller call context.
    /// </remarks>
    /// </summary>
    public class CallContextTraceAccessor : ITraceAccessor
    {
        private const string ContextKey = "server-trace";

        public void SaveTrace(Trace trace)
        {
            CallContext.LogicalSetData(ContextKey, trace);
        }

        public Trace GetTrace()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace as Trace;
        }

        public bool HasTrace()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace is Trace;
        }
    }
}
