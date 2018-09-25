using System;

namespace Zipkin.NET.Logging
{
    public class ConsoleTracingLogger : ITracingLogger
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
