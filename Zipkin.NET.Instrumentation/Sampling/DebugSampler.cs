namespace Zipkin.NET.Instrumentation.Sampling
{
    public class DebugSampler : ISampler
    {
        public bool IsSampled(TraceContext traceContext)
        {
            return true;
        }
    }
}
