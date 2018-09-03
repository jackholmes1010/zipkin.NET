using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Instrumentation
{
    public class TraceContextAccessor : ITraceContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TraceContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public TraceContext Context
        {
            get => _httpContextAccessor.HttpContext.Items["server-trace"] as TraceContext;
            set => _httpContextAccessor.HttpContext.Items["server-trace"] = value;
        }
    }
}
