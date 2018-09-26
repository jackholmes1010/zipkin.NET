namespace Zipkin.NET
{
    /// <summary>
    /// Used to access the current <see cref="TraceContext"/> object across the application.
    /// <remarks>
    /// Implementations should use some persistent store across the 
    /// context of the current request such as the HttpContext or CallContext.
    /// </remarks>
    /// </summary>
    public interface ITraceContextAccessor
    {
        /// <summary>
        /// Stores a <see cref="TraceContext"/> in the context of the current request.
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// </param>
        void SaveTrace(TraceContext traceContext);

        /// <summary>
        /// Retrieves a <see cref="TraceContext"/> from the context of the current request.
        /// </summary>
        /// <returns>
        /// The <see cref="TraceContext"/>.
        /// </returns>
        TraceContext GetTrace();

        /// <summary>
        /// Checks if a <see cref="TraceContext"/> is stored in the context of the current request.
        /// </summary>
        /// <returns>
        /// True if a trace context is stored, else false.
        /// </returns>
        bool HasTrace();
    }
}
