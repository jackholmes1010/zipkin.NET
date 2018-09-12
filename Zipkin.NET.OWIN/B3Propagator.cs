using Microsoft.Owin;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;

namespace Zipkin.NET.OWIN
{
    /// <summary>
    /// Injects trace context into an <see cref="IOwinContext"/>.
    /// </summary>
    public class B3Propagator : IPropagator<IOwinContext>
    {
        /// <summary>
        /// Adds X-B3 header values to an outgoing HTTP request.
        /// </summary>
        /// <param name="context">
        /// The current OWIN context.
        /// </param>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/> which contains trace ID context for the current trace.
        /// </param>
        /// <returns></returns>
        public IOwinContext Inject(IOwinContext context, TraceContext traceContext)
        {
            context.Request.Headers.Add(B3HeaderConstants.TraceId, new []{ traceContext.TraceId });
            context.Request.Headers.Add(B3HeaderConstants.SpanId, new []{ traceContext.SpanId });
            context.Request.Headers.Add(B3HeaderConstants.ParentSpanId, new []{ traceContext.ParentSpanId });
            context.Request.Headers.Add(B3HeaderConstants.Sampled, new []{ traceContext.Sampled == true ? "1" : "0" });
            context.Request.Headers.Add(B3HeaderConstants.Flags, new []{ traceContext.Debug == true ? "1" : "0" });
            return context;
        }
    }
}
