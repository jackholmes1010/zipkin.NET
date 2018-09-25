using System.Web;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the <see cref="TraceContext"/> in the <see cref="HttpContext.Current"/>.
    /// </summary>
    public class SystemWebHttpContextTraceContextAccessor : ITraceContextAccessor
    {
        private const string ContextKey = "server-trace";

        public void SaveTrace(TraceContext traceContext)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[ContextKey] = traceContext;
        }

        public TraceContext GetTrace()
        {
            return HttpContext.Current?.Items[ContextKey] as TraceContext;
        }

        public bool HasTrace()
        {
            return HttpContext.Current?.Items[ContextKey] is TraceContext;
        }
    }
}
