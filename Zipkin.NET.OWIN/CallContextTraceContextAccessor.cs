using System;
using System.Runtime.Remoting.Messaging;
using Zipkin.NET.Instrumentation;

namespace Zipkin.NET.OWIN
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
        /// Get the <see cref="TraceContext"/> from the <see cref="CallContext"/>.
        /// </summary>
        public TraceContext Context
        {
            get
            {
                var context = CallContext.LogicalGetData(ContextKey);

                if (context == null)
                    throw new Exception("Unable to retrieve trace context from the CallContext.");

                var traceContext = context as TraceContext;
                return traceContext;
            }
            set
            {
                CallContext.LogicalSetData(ContextKey, value);
            }
        }
    }
}
