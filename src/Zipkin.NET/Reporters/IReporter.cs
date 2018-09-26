using System.Threading.Tasks;
using Zipkin.NET.Models;

namespace Zipkin.NET.Reporters
{
    /// <summary>
    /// Reporters sends spans recorded by instrumentation out of process.
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Schedules the span to be sent onto the transport.
        /// </summary>
        /// <param name="span">
        /// The trace to be reported.
        /// </param>
        Task ReportAsync(Span span);
    }
}
