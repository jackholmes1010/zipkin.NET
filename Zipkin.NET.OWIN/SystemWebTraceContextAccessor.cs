using System;
using System.Web;
using Zipkin.NET.Instrumentation;
using TraceContext = Zipkin.NET.Instrumentation.TraceContext;

namespace Zipkin.NET.OWIN
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
			get
			{
				if (HttpContext.Current == null)
					throw new Exception("Unable to retrieve trace context, HttpContext.Current is null.");

				var traceContext = HttpContext.Current.Items[ContextKey];

				if (traceContext == null)
					throw new Exception("Unable to retrieve trace context from HttpContext.Current.Items.");

				return traceContext as TraceContext;
			}
			set
			{
				if (HttpContext.Current == null)
					throw new Exception("Unable to store trace context on the HttpContext. HttpContext.Current is null.");

				HttpContext.Current.Items[ContextKey] = value;
			}
		}
    }
}
