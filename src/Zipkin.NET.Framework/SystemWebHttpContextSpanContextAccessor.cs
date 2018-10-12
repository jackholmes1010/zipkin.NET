using System.Web;

namespace Zipkin.NET.Framework
{
    /// <summary>
    /// Stores the <see cref="SpanContext"/> in the <see cref="HttpContext.Current"/>.
    /// </summary>
    public class SystemWebHttpContextSpanContextAccessor : ISpanContextAccessor
    {
        private const string ContextKey = "server-trace";

        /// <summary>
        /// Stores a <see cref="SpanContext"/> on the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="spanContext">
        /// The <see cref="SpanContext"/>.
        /// </param>
        public void SaveContext(SpanContext spanContext)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[ContextKey] = spanContext;
        }

        /// <summary>
        /// Retrieves a <see cref="SpanContext"/> from the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="SpanContext"/>.
        /// </returns>
        public SpanContext GetContext()
        {
            return HttpContext.Current?.Items[ContextKey] as SpanContext;
        }

        /// <summary>
        /// Checks if a <see cref="SpanContext"/> is stored on the <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// True if a span context is stored, else false.
        /// </returns>
        public bool HasContext()
        {
            return HttpContext.Current?.Items[ContextKey] is SpanContext;
        }
    }
}
