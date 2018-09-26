using Microsoft.Extensions.Logging;
using Zipkin.NET.Logging;

namespace Zipkin.NET.Core.Logging
{
    /// <summary>
    /// .NET Core <see cref="IInstrumentationLogger"/> used to log
    /// instrumentation errors using a <see cref="ILogger{T}"/>.
    /// </summary>
    public class CoreInstrumentationLogger : IInstrumentationLogger
    {
        private readonly ILogger<CoreInstrumentationLogger> _logger;

        public CoreInstrumentationLogger(ILogger<CoreInstrumentationLogger> logger)
        {
            _logger = logger;
        }

        public void WriteLog(string log)
        {
            _logger.LogDebug(log);
        }

        public void WriteError(string log)
        {
            _logger.LogError(log);
        }
    }
}
