using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// Retrieves and saves the current <see cref="TraceContext"/> to and from the <see cref="HttpContext"/>.
    /// </summary>
    public class HttpContextTraceContextAccessor : ITraceContextAccessor
    {
	    private const string ContextKey = "server-trace";
		private readonly IHttpContextAccessor _httpContextAccessor;

		public HttpContextTraceContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// The <see cref="TraceContext"/> stored on the <see cref="HttpContext"/>.
        /// </summary>
        public TraceContext Context
        {
            get => _httpContextAccessor.HttpContext.Items[ContextKey] as TraceContext;
            set => _httpContextAccessor.HttpContext.Items[ContextKey] = value;
        }

	    public bool HasContext()
	    {
		   return _httpContextAccessor.HttpContext?.Items[ContextKey] is TraceContext;
	    }
    }
}
