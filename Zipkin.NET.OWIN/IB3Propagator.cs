using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Owin;
using Zipkin.NET.Instrumentation;

namespace Zipkin.NET.OWIN
{
	/// <summary>
	/// Extracts and injects X-B3 headers from/to HTTP requests.
	/// </summary>
	public interface IB3Propagator
	{
		/// <summary>
		/// Extract X-B3 trace headers from the HTTP request associated with the given <see cref="HttpContext"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IOwinContext"/> to extract headers from.
		/// </param>
		/// <returns>
		/// A <see cref="TraceContext"/> which contains values for the X-B3 headers.
		/// </returns>
		TraceContext Extract(IOwinContext context);

		/// <summary>
		/// Inject X-B3 trace headers to the given HTTP request.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IOwinContext"/> to inject headers onto.
		/// </param>
		/// <param name="traceContext">
		/// The <see cref="TraceContext"/> containing values for the X-B3 headers.
		/// </param>
		/// <returns>
		/// The given <see cref="HttpRequestMessage"/>.
		/// </returns>
		IOwinContext Inject(IOwinContext context, TraceContext traceContext);
	}
}
