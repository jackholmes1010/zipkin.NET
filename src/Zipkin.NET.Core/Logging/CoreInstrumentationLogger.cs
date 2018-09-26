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

        /// <summary>
        /// Construct a new <see cref="CoreInstrumentationLogger"/>.
        /// </summary>
        /// <param name="logger">
        /// A .NET Core <see cref="ILogger{T}"/>.
        /// </param>
        public CoreInstrumentationLogger(ILogger<CoreInstrumentationLogger> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Write a debug log.
        /// </summary>
        /// <param name="log">
        /// The log message.
        /// </param>
        public void WriteLog(string log)
        {
            _logger.LogDebug(log);
        }

        /// <summary>
        /// Write an error log.
        /// </summary>
        /// <param name="log">
        /// The log message.
        /// </param>
        public void WriteError(string log)
        {
            _logger.LogError(log);
        }
    }
}
