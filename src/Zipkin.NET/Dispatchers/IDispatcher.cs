using Zipkin.NET.Models;

namespace Zipkin.NET.Dispatchers
{
    public interface IDispatcher
    {
        void Dispatch(Span span, TraceContext traceContext);
    }
}
