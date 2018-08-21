using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Instrumentation
{
    public class TraceContext : ITraceContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TraceContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public string CurrentSpanId
        {
            get => _httpContextAccessor.HttpContext.Items["current-spanId"] as string;
            set => _httpContextAccessor.HttpContext.Items["current-spanId"] = value;
        }

        public string CurrentTraceId
        {
            get => _httpContextAccessor.HttpContext.Items["current-traceId"] as string;
            set => _httpContextAccessor.HttpContext.Items["current-traceId"] = value;
        }
    }
}
