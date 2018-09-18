namespace Zipkin.NET
{
    /// <summary>
    /// Used to access the current <see cref="Trace"/> object across the application.
    /// <remarks>
    /// Implementations should use some persistent store across the 
    /// context of the current request such as the HttpContext or CallContext.
    /// </remarks>
    /// </summary>
    public interface ITraceAccessor
    {
        void SaveTrace(Trace trace);
        Trace GetTrace();
        bool HasTrace();
    }
}
