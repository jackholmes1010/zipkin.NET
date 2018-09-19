using System.Web;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the <see cref="Trace"/> in the <see cref="HttpContext.Current"/>.
    /// </summary>
    public class SystemWebHttpContextTraceAccessor : ITraceAccessor
    {
        private const string ContextKey = "server-trace";

        public void SaveTrace(Trace trace)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[ContextKey] = trace;
        }

        public Trace GetTrace()
        {
            return HttpContext.Current?.Items[ContextKey] as Trace;
        }

        public bool HasTrace()
        {
            return HttpContext.Current?.Items[ContextKey] is Trace;
        }
    }
}
