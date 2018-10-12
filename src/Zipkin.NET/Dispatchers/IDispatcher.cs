using Zipkin.NET.Models;

namespace Zipkin.NET.Dispatchers
{
    /// <summary>
    /// Dispatches completed spans to reporters.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatches a completed span.
        /// </summary>
        /// <param name="span">
        /// The completed span.
        /// </param>
        void Dispatch(Span span);
    }
}
