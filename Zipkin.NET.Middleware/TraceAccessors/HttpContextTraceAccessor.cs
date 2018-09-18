using System;
using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Middleware.TraceAccessors
{
    /// <summary>
    /// <see cref="ITraceAccessor"/> backed by the <see cref="HttpContext"/>.
    /// </summary>
    /// <inheritdoc />
    public class HttpContextTraceAccessor : ITraceAccessor
    {
        private const string ContextKey = "server-trace";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTraceAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor 
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void SaveTrace(Trace trace)
        {
            _httpContextAccessor.HttpContext.Items[ContextKey] = trace;
        }

        public Trace GetTrace()
        {
            return _httpContextAccessor.HttpContext.Items[ContextKey] as Trace;
        }

        public bool HasTrace()
        {
            return _httpContextAccessor.HttpContext?.Items[ContextKey] is Trace;
        }
    }
}
