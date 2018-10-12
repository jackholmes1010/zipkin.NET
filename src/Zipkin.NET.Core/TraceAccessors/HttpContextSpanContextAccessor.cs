using System;
using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Core.TraceAccessors
{
    /// <summary>
    /// <see cref="ISpanContextAccessor"/> backed by the <see cref="HttpContext"/>.
    /// </summary>
    /// <inheritdoc />
    public class HttpContextSpanContextAccessor : ISpanContextAccessor
    {
        private const string ContextKey = "server-trace";
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Construct a new <see cref="HttpContextAccessor"/>.
        /// </summary>
        /// <param name="httpContextAccessor">
        /// An <see cref="IHttpContextAccessor"/> used to retrieve the 
        /// <see cref="HttpContext"/> used to store span context.
        /// </param>
        public HttpContextSpanContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor 
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Stores a <see cref="SpanContext"/> on the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="spanContext">
        /// The <see cref="SpanContext"/>.
        /// </param>
        public void SaveContext(SpanContext spanContext)
        {
            _httpContextAccessor.HttpContext.Items[ContextKey] = spanContext;
        }

        /// <summary>
        /// Retrieves a <see cref="SpanContext"/> from the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="SpanContext"/>.
        /// </returns>
        public SpanContext GetContext()
        {
            return _httpContextAccessor.HttpContext.Items[ContextKey] as SpanContext;
        }

        /// <summary>
        /// Checks if a <see cref="SpanContext"/> is stored on the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// True if a span context is stored, else false.
        /// </returns>
        public bool HasContext()
        {
            return _httpContextAccessor.HttpContext?.Items[ContextKey] is SpanContext;
        }
    }
}
