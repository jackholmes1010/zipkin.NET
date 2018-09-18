using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Middleware
{
	public class TracingMiddleware : IMiddleware
	{
		private readonly string _applicationName;
		private readonly IExtractor<HttpRequest> _extractor;
		private readonly ITraceAccessor _traceAccessor;
		private readonly IReporter _reporter;

		public TracingMiddleware(
			string applicationName,
			IExtractor<HttpRequest> extractor,
			ITraceAccessor traceAccessor,
			IReporter reporter)
		{
			_applicationName = applicationName;
			_extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
			_traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
			_reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			var parentSpan = _extractor.Extract(context.Request);
			var trace = new Trace(parentSpan.TraceId, parentSpan.Id);
			var spanBuilder = trace.GetSpanBuilder();

			spanBuilder
				.Tag("host", context.Request.Host.Value)
				.Tag("resource", context.Request.Path.Value)
				.Tag("method", context.Request.Method)
				.WithLocalEndpoint(new Endpoint
				{
					ServiceName = _applicationName
				})
				.Start();

			_traceAccessor.SaveTrace(trace);

			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				spanBuilder.Error(ex.Message);
			}
			finally
			{
				spanBuilder.End();
				_reporter.Report(spanBuilder.Build());
			}
		}
	}
}
