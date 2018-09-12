using System.Linq;
using Microsoft.Owin;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.OWIN
{
    /// <summary>
    /// Extracts and adds X-B3 header values to HTTP requests.
    /// </summary>
    public class B3Propagator : IPropagator<IOwinContext, IOwinContext>
    {
        private readonly ISampler _sampler;

        public B3Propagator(ISampler sampler)
        {
            _sampler = sampler;
        }

        /// <summary>
        /// Extracts the X-B3 trace ID header values from the request.
        /// </summary>
        /// <param name="context">
        /// The current OWIN context.
        /// </param>
        /// <returns>
        /// A <see cref="TraceContext"/> containing the header values.
        /// </returns>
        public TraceContext Extract(IOwinContext context)
        {
            string traceId = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                traceId = value.FirstOrDefault();
            }

            string spanId = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                spanId = value.FirstOrDefault();
            }

            bool? sampled = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
            {
                sampled = value.FirstOrDefault() == "1";
            }

            bool? debug = null;
            if (context.Request.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                debug = value.FirstOrDefault() == "1";
            }

            return new TraceContext(_sampler)
            {
                TraceId = traceId,
                SpanId = spanId,
                Sampled = sampled,
                Debug = debug
            };
        }

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
