using Zipkin.NET.Models;

namespace Zipkin.NET.Dispatchers
{
    /// <summary>
    /// Dispatch a completed span to available reporters.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatch a completed span to available reporters.
        /// </summary>
        /// <param name="span">
        /// The completed <see cref="Span"/>.
        /// </param>
        void Dispatch(Span span);
    }
}
