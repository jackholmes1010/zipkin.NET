using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;

namespace Zipkin.NET.Middleware
{
    /// <summary>
    /// Retrieves and saves the current <see cref="TraceContext"/> to and from the <see cref="HttpContext"/>.
    /// </summary>
    public class HttpContextTraceContextAccessor : ITraceContextAccessor
    {
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
            get => _httpContextAccessor.HttpContext.Items["server-trace"] as TraceContext;
            set => _httpContextAccessor.HttpContext.Items["server-trace"] = value;
        }
    }
}
