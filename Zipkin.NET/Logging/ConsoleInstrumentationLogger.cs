using System;

namespace Zipkin.NET.Logging
{
    /// <summary>
    /// Log instrumentation to log errors.
    /// </summary>
    public class ConsoleInstrumentationLogger : IInstrumentationLogger
    {
        public void WriteLog(string log)
        {
            Console.WriteLine(log);
        }

        public void WriteError(string log)
        {
            Console.Error.WriteLine(log);
        }
    }
}
