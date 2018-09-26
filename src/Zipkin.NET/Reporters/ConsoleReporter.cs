using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zipkin.NET.Models;

namespace Zipkin.NET.Reporters
{
    /// <summary>
    /// Writes completed spans to the console.
    /// </summary>
    public class ConsoleReporter : IReporter
    {
        /// <summary>
        /// Write the completed span to the console.
        /// </summary>
        /// <param name="span">
        /// The completed span.
        /// </param>
        /// <returns>
        /// A completed <see cref="Task" />.
        /// </returns>
        public Task ReportAsync(Span span)
        {
            var serializedSpan = JsonConvert.SerializeObject(span, Formatting.Indented);
            Console.WriteLine(serializedSpan);
            return Task.CompletedTask;
        }
    }
}
