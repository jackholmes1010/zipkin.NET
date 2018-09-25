using Microsoft.Extensions.Logging;
using Zipkin.NET.Logging;

namespace Zipkin.NET.Middleware.Logging
{
    public class CoreTracingLogger : ITracingLogger
    {
        private readonly ILogger<CoreTracingLogger> _logger;

        public CoreTracingLogger(ILogger<CoreTracingLogger> logger)
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
