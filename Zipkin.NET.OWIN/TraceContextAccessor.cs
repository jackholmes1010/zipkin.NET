using System.Web;
using Zipkin.NET.Instrumentation;
using TraceContext = Zipkin.NET.Instrumentation.TraceContext;

namespace Zipkin.NET.OWIN
{
    public class TraceContextAccessor : ITraceContextAccessor
    {
        public TraceContext Context
        {
            get => HttpContext.Current.Items["server-trace"] as TraceContext;
            set => HttpContext.Current.Items["server-trace"] = value;
        }
    }
}
