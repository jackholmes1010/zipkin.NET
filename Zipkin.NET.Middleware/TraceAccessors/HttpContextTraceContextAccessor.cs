using System;
using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Middleware.TraceAccessors
{
    /// <summary>
    /// <see cref="ITraceContextAccessor"/> backed by the <see cref="HttpContext"/>.
    /// </summary>
    /// <inheritdoc />
    public class HttpContextTraceContextAccessor : ITraceContextAccessor
    {
        private const string ContextKey = "server-trace";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTraceContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor 
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void SaveTrace(TraceContext traceContext)
        {
            _httpContextAccessor.HttpContext.Items[ContextKey] = traceContext;
        }

        public TraceContext GetTrace()
        {
            return _httpContextAccessor.HttpContext.Items[ContextKey] as TraceContext;
        }

        public bool HasTrace()
        {
            return _httpContextAccessor.HttpContext?.Items[ContextKey] is TraceContext;
        }
    }
}
