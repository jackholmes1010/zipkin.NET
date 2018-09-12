namespace Zipkin.NET.Instrumentation.Sampling
{
    public class DebugSampler : ISampler
    {
        public bool IsSampled(Trace trace)
        {
            return true;
        }
    }
}
