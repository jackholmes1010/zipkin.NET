namespace Zipkin.NET.Instrumentation
{
    public interface ITraceContextAccessor
    {
        TraceContext Context { get; set; }
    }
}
