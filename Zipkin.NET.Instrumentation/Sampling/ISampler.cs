namespace Zipkin.NET.Instrumentation.Sampling
{
    /// <summary>
    /// Samplers are used to make sampling decisions for whether
    /// a trace should be reported. If a sampling decision has
    /// already been made by an upstream service, or the debug
    /// flag is set, reporters should respect the existing decision.
    /// </summary>
    public interface ISampler
    {
        /// <summary>
        /// Decide if a given trace should be sampled.
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// </param>
        /// <returns>
        /// True if the trace should be sampled.
        /// </returns>
        bool IsSampled(TraceContext traceContext);
    }
}
