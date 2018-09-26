using System.Web;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the <see cref="TraceContext"/> in the <see cref="HttpContext.Current"/>.
    /// </summary>
    public class SystemWebHttpContextTraceContextAccessor : ITraceContextAccessor
    {
        private const string ContextKey = "server-trace";

        /// <summary>
        /// Stores a <see cref="TraceContext"/> on the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// </param>
        public void SaveTrace(TraceContext traceContext)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[ContextKey] = traceContext;
        }

        /// <summary>
        /// Retrieves a <see cref="TraceContext"/> from the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext GetTrace()
        {
            return HttpContext.Current?.Items[ContextKey] as TraceContext;
        }

        /// <summary>
        /// Checks if a <see cref="TraceContext"/> is stored on the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// True if a trace context is stored, else false.
        /// </returns>
        public bool HasTrace()
        {
            return HttpContext.Current?.Items[ContextKey] is TraceContext;
        }
    }
}
