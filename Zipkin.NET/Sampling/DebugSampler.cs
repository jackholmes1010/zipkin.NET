namespace Zipkin.NET.Sampling
{
    /// <summary>
    /// A <see cref="Sampler"/> that always samples.
    /// </summary>
    public class DebugSampler : Sampler
    {
        /// <summary>
        /// Always sample.
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// </param>
        /// <returns>
        /// True.
        /// </returns>
        protected override bool MakeSamplingDecision(TraceContext traceContext)
        {
            return true;
        }
    }
}
