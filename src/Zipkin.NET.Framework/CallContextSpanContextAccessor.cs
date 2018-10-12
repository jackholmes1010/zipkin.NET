using System.Runtime.Remoting.Messaging;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the current <see cref="SpanContext"/> in the logical <see cref="CallContext"/>.
    /// <remarks>
    /// This will not work if an application is hosted using IIS since the 
    /// middleware's context is not shared with the controller call context.
    /// </remarks>
    /// </summary>
    public class CallContextSpanContextAccessor : ISpanContextAccessor
    {
        private const string ContextKey = "server-trace";

        /// <summary>
        /// Stores a <see cref="SpanContext"/> on the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="spanContext">
        /// The <see cref="SpanContext"/>.
        /// </param>
        public void SaveContext(SpanContext spanContext)
        {
            CallContext.LogicalSetData(ContextKey, spanContext);
        }

        /// <summary>
        /// Retrieves a <see cref="SpanContext"/> from the <see cref="CallContext"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="SpanContext"/>.
        /// </returns>
        public SpanContext GetContext()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace as SpanContext;
        }

        /// <summary>
        /// Checks if a <see cref="SpanContext"/> is stored on the <see cref="CallContext"/>.
        /// </summary>
        /// <returns>
        /// True if a span context is stored, else false.
        /// </returns>
        public bool HasContext()
        {
            var trace = CallContext.LogicalGetData(ContextKey);
            return trace is SpanContext;
        }
    }
}
