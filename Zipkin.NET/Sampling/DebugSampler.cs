namespace Zipkin.NET.Sampling
{
    public class DebugSampler : ISampler
    {
        public bool IsSampled(TraceContext traceContext)
        {
            return true;
        }
    }
}
