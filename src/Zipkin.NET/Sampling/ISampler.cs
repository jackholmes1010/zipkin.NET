namespace Zipkin.NET.Sampling
{
    public interface ISampler
    {
        bool IsSampled(TraceContext traceContext);
    }
}
