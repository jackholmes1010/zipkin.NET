using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// Extracts and injects X-B3 headers from/to HTTP requests.
    /// </summary>
    public interface IB3Propagator
    {
        /// <summary>
        /// Extract X-B3 trace headers from the HTTP request associated with the given <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">
        /// The current <see cref="HttpContext"/>.
        /// </param>
        /// <returns>
        /// A <see cref="TraceContext"/> which contains values for the X-B3 headers.
        /// </returns>
        TraceContext Extract(HttpContext httpContext);

        /// <summary>
        /// Inject X-B3 trace headers to the given HTTP request.
        /// </summary>
        /// <param name="request">
        /// The <see cref="HttpRequestMessage"/> onto which we wish to inject headers.
        /// </param>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/> containing values for the X-B3 headers.
        /// </param>
        /// <returns>
        /// The given <see cref="HttpRequestMessage"/>.
        /// </returns>
        HttpRequestMessage Inject(HttpRequestMessage request, TraceContext traceContext);
    }
}
