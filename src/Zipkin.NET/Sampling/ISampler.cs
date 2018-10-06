namespace Zipkin.NET.Sampling
{
    /// <summary>
    /// Samplers are used to decide if a trace should be sampled.
    /// </summary>
    public interface ISampler
    {
        /// <summary>
        /// Decide if a trace should be sampled.
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
