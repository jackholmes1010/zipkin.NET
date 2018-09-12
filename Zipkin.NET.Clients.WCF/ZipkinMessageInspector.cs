using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Clients.WCF
{
	/// <summary>
	/// Message inspector responsible for adding Zipkin X-B3 HTTP headers to outgoing WCF requests.
	/// </summary>
	/// <inheritdoc />
	public class ZipkinMessageInspector : IClientMessageInspector
	{
		private readonly string _applicationName;
		private readonly IReporter _reporter;
		private readonly ITraceContextAccessor _traceContextAccessor;

		private ClientTrace _trace;

		public ZipkinMessageInspector(
			string applicationName,
			IReporter reporter,
			ITraceContextAccessor traceContextAccessor)
		{
			_applicationName = applicationName;
			_reporter = reporter;
			_traceContextAccessor = traceContextAccessor;
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

			_trace = new ClientTrace(
				traceContext,
				request.Headers.Action,
				remoteEndpoint: new Endpoint
				{
					ServiceName = _applicationName
				});

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
