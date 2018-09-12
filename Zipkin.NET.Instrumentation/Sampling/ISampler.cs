namespace Zipkin.NET.Instrumentation.Sampling
{
    public interface ISampler
    {
        bool IsSampled(string traceId);
    }
}
