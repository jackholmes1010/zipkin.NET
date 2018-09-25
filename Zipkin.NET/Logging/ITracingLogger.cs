namespace Zipkin.NET.Logging
{
    /// <summary>
    /// Used by tracing instrumentation to log errors.
    /// </summary>
    public interface ITracingLogger
    {
        /// <summary>
        /// Write a standard debug log.
        /// </summary>
        /// <param name="log">
        /// The log message.
        /// </param>
        void WriteLog(string log);

        /// <summary>
        /// Write an error log.
        /// </summary>
        /// <param name="log">
        /// The log message.
        /// </param>
        void WriteError(string log);
    }
}
