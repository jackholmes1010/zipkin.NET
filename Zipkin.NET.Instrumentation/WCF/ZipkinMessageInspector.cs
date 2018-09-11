using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Instrumentation.WCF
{
	/// <summary>
	/// Message inspector responsible for adding Zipkin X-B3 HTTP headers to outgoing WCF requests.
	/// </summary>
	/// <inheritdoc />
	public class ZipkinMessageInspector : IClientMessageInspector
	{
		private readonly ITraceContextAccessor _traceContextAccessor;
		private readonly IReporter _reporter;

		private ClientTrace _trace;

		public ZipkinMessageInspector(
			ITraceContextAccessor traceContextAccessor, 
			IReporter reporter)
		{
			_traceContextAccessor = traceContextAccessor;
			_reporter = reporter;
		}

		/// <inheritdoc />
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var traceContext = _traceContextAccessor.Context.NewChild();

			var httpRequest = ExtractHttpRequest(request);
			httpRequest.Headers.Add(B3HeaderConstants.TraceId, traceContext.TraceId);
			httpRequest.Headers.Add(B3HeaderConstants.SpanId, traceContext.SpanId);
			httpRequest.Headers.Add(B3HeaderConstants.ParentSpanId, traceContext.ParentSpanId);
			httpRequest.Headers.Add(B3HeaderConstants.Sampled, traceContext.Sampled == true ? "1" : "0");
			httpRequest.Headers.Add(B3HeaderConstants.Flags, traceContext.Debug == true ? "1" : "0");

			_trace = new ClientTrace(traceContext, "soap");
			_trace.Start();

			return null;
		}

		/// <inheritdoc />
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			_trace.End();
			_reporter.Report(_trace.Span);
		}

		private static HttpRequestMessageProperty ExtractHttpRequest(Message wcfMessage)
		{
			HttpRequestMessageProperty httpRequest;
			if (wcfMessage.Properties.TryGetValue(HttpRequestMessageProperty.Name, out var requestObject))
			{
				httpRequest = requestObject as HttpRequestMessageProperty;
			}
			else
			{
				httpRequest = new HttpRequestMessageProperty();
				wcfMessage.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
			}

			return httpRequest;
		}
	}
}
