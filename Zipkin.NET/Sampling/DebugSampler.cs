namespace Zipkin.NET.Sampling
{
    public class DebugSampler : ISampler
    {
        public bool IsSampled(Trace trace)
        {
            return true;
        }
    }
}
