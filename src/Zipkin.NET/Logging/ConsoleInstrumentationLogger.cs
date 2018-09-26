using System;

namespace Zipkin.NET.Logging
{
    /// <summary>
    /// An <see cref="IInstrumentationLogger"/>
    /// used to log instrumentation errors to the console.
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
