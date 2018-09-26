using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Core.Reporters
{
    /// <summary>
    /// Log completed spans using a .NET Core <see cref="ILogger{T}"/>.
    /// </summary>
    public class LoggerReporter : IReporter
    {
        private readonly ILogger<LoggerReporter> _logger;

        /// <summary>
        /// Construct a new <see cref="LoggerReporter"/>.
        /// </summary>
        /// <param name="logger">
        /// A .NET Core <see cref="ILogger{T}"/>.
        /// </param>
        public LoggerReporter(ILogger<LoggerReporter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Log the completed span.
        /// </summary>
        /// <param name="span">
        /// The completed <see cref="Span"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/>.
        /// </returns>
        public Task ReportAsync(Span span)
        {
            var serializedSpan = JsonConvert.SerializeObject(span, Formatting.Indented);
            _logger.LogInformation(serializedSpan);
            return Task.CompletedTask;
        }
    }
}
