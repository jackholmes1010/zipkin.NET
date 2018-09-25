using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Middleware.Reporters
{
    public class LoggerReporter : IReporter
    {
        private readonly ILogger<LoggerReporter> _logger;

        public LoggerReporter(ILogger<LoggerReporter> logger)
        {
            _logger = logger;
        }

        public Task ReportAsync(Span span)
        {
            var serializedSpan = JsonConvert.SerializeObject(span, Formatting.Indented);
            _logger.LogInformation(serializedSpan);
            return Task.CompletedTask;
        }
    }
}
