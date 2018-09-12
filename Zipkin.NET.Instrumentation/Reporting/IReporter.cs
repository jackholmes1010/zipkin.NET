using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation.Reporting
{
    /// <summary>
    /// Reporters sends spans recorded by instrumentation out of process.
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Schedules the span to be sent onto the transport.
        /// </summary>
        /// <param name="trace">
        /// The trace to be reported.
        /// </param>
        void Report(Trace trace);
    }
}
