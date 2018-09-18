//using System.ServiceModel.Channels;
//using Zipkin.NET.Instrumentation;
//using Zipkin.NET.Instrumentation.Constants;
//using Zipkin.NET.Instrumentation.Propagation;

//namespace Zipkin.NET.Clients.WCF
//{
//    /// <summary>
//    /// Injects trace context into an <see cref="HttpRequestMessageProperty"/>.
//    /// </summary>
//    public class HttpRequestMessagePropertyB3Propagator : IPropagator<HttpRequestMessageProperty>
//    {
//        /// <summary>
//        /// Adds X-B3 header values to an outgoing HTTP request.
//        /// </summary>
//        /// <param name="httpRequest">
//        /// The request to add headers to.
//        /// </param>
//        /// <param name="context">
//        /// The <see cref="TraceContext"/> which contains trace ID context for the current trace.
//        /// </param>
//        /// <returns>
//        /// The HTTP request with trace headers.
//        /// </returns>
//        public HttpRequestMessageProperty Inject(HttpRequestMessageProperty httpRequest, TraceContext context)
//        {
//            httpRequest.Headers.Add(B3HeaderConstants.TraceId, context.TraceId);
//            httpRequest.Headers.Add(B3HeaderConstants.SpanId, context.SpanId);
//            httpRequest.Headers.Add(B3HeaderConstants.ParentSpanId, context.ParentSpanId);
//            httpRequest.Headers.Add(B3HeaderConstants.Sampled, context.Sampled == true ? "1" : "0");
//            httpRequest.Headers.Add(B3HeaderConstants.Flags, context.Debug == true ? "1" : "0");
//            return httpRequest;
//        }
//    }
//}
