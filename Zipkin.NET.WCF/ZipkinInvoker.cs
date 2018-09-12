using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.WCF
{
	public class ZipkinInvoker : IOperationInvoker
	{
		private readonly string _applicationName;
		private readonly IOperationInvoker _originalInvoker;
		private readonly IReporter _reporter;
		private readonly ITraceIdentifierGenerator _traceIdGenerator;
		private readonly ISampler _sampler;

		public ZipkinInvoker(
			string applicationName,
			IOperationInvoker originalInvoker,
			IReporter reporter, 
			ITraceIdentifierGenerator traceIdGenerator, 
			ISampler sampler)
		{
			_applicationName = applicationName;
			_originalInvoker = originalInvoker;
			_reporter = reporter;
			_traceIdGenerator = traceIdGenerator;
			_sampler = sampler;
		}

		public bool IsSynchronous => _originalInvoker.IsSynchronous;

		public object[] AllocateInputs() { return _originalInvoker.AllocateInputs(); }

		public object Invoke(object instance, object[] inputs, out object[] outputs)
		{
			var headers = WebOperationContext.Current?.IncomingRequest.Headers;

			var traceContext = new TraceContext(_traceIdGenerator, _sampler)
			{
				TraceId = headers?[B3HeaderConstants.TraceId],
				SpanId = headers?[B3HeaderConstants.SpanId],
				Debug = headers?[B3HeaderConstants.Flags] == "1"
			};

			var sampled = headers?[B3HeaderConstants.Sampled];

			// Sampled should not be set if not flag
			// is provided by the upstream service.
			if (sampled != null)
			{
				traceContext.Sampled = sampled == "1";
			}

			var trace = new ServerTrace(
				traceContext, 
				"soap", 
				localEndpoint: new Endpoint
				{
					ServiceName = _applicationName
				});

			trace.Start();

			// Do stuff before call
			var res = _originalInvoker.Invoke(instance, inputs, out outputs);
			trace.End();
			_reporter.Report(trace.Span);

			// stuff after call
			return res;
		}

		public IAsyncResult InvokeBegin(object instance, object[] inputs,
			AsyncCallback callback, object state)
		{
			//Do stuff before async call
			var res = _originalInvoker.InvokeBegin(instance, inputs, callback, state);
			return res;
		}

		public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
		{
			var res = _originalInvoker.InvokeEnd(instance, out outputs, result);
			// Do stuff after async call
			return res;
		}
	}
}