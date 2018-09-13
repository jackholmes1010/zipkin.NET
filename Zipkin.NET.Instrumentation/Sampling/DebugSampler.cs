namespace Zipkin.NET.Instrumentation.Sampling
{
    /// <summary>
    /// Debug sampler samples all traces.
    /// </summary>
    public class DebugSampler : ISampler
    {
        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <param name="traceContext">
        /// The current <see cref="TraceContext"/>.
        /// </param>
        /// <returns>
        /// True.
        /// </returns>
        public bool IsSampled(TraceContext traceContext)
        {
            return true;
        }
    }
}
