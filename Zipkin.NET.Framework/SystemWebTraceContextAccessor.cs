using System;
using System.Web;
using Zipkin.NET.Instrumentation;
using TraceContext = Zipkin.NET.Instrumentation.TraceContext;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the <see cref="TraceContext"/> in the <see cref="HttpContext.Current"/>.
    /// </summary>
    public class SystemWebTraceContextAccessor : ITraceContextAccessor
    {
        private const string ContextKey = "server-trace";

        /// <summary>
        /// Get the <see cref="TraceContext"/> from the <see cref="HttpContext.Current"/>.
        /// </summary>
        public TraceContext Context
        {
            get => HttpContext.Current?.Items[ContextKey] as TraceContext;
	        set
	        {
		        if (HttpContext.Current == null)
			        return;

                HttpContext.Current.Items[ContextKey] = value;
            }
        }

	    public bool HasContext()
	    {
		    return HttpContext.Current?.Items[ContextKey] is TraceContext;
	    }
    }
}
