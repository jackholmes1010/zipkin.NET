using System;
using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Core.TraceAccessors
{
    /// <summary>
    /// <see cref="ITraceContextAccessor"/> backed by the <see cref="HttpContext"/>.
    /// </summary>
    /// <inheritdoc />
    public class HttpContextTraceContextAccessor : ITraceContextAccessor
    {
        private const string ContextKey = "server-trace";
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Construct a new <see cref="HttpContextAccessor"/>.
        /// </summary>
        /// <param name="httpContextAccessor">
        /// An <see cref="IHttpContextAccessor"/> used to retrieve the 
        /// <see cref="HttpContext"/> used to store trace context.
        /// </param>
        public HttpContextTraceContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor 
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Stores a <see cref="TraceContext"/> on the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// </param>
        public void SaveTrace(TraceContext traceContext)
        {
            _httpContextAccessor.HttpContext.Items[ContextKey] = traceContext;
        }

        /// <summary>
        /// Retrieves a <see cref="TraceContext"/> from the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext GetTrace()
        {
            return _httpContextAccessor.HttpContext.Items[ContextKey] as TraceContext;
        }

        /// <summary>
        /// Checks if a <see cref="TraceContext"/> is stored on the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// True if a trace context is stored, else false.
        /// </returns>
        public bool HasTrace()
        {
            return _httpContextAccessor.HttpContext?.Items[ContextKey] is TraceContext;
        }
    }
}
