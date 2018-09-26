using System;

namespace Zipkin.NET.Logging
{
    /// <summary>
    /// An <see cref="IInstrumentationLogger"/>
    /// used to log instrumentation errors to the console.
    /// </summary>
    public class ConsoleInstrumentationLogger : IInstrumentationLogger
    {
        /// <summary>
        /// Write the log to the console.
        /// </summary>
        /// <param name="log">
        /// The log message.
        /// </param>
        public void WriteLog(string log)
        {
            Console.WriteLine(log);
        }

        /// <summary>
        /// Write the log to the error stream.
        /// </summary>
        /// <param name="log">
        /// The log message.
        /// </param>
        public void WriteError(string log)
        {
            Console.Error.WriteLine(log);
        }
    }
}
