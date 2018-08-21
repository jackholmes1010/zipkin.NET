namespace Zipkin.NET.Instrumentation
{
    public interface ITraceContext
    {
        string CurrentSpanId { get; set; }
        string CurrentTraceId { get; set; }
    }
}
