using System;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.WCF.Demo
{
	public class ZipkinInvoker : IOperationInvoker
	{
		private readonly IOperationInvoker _originalInvoker;

		public ZipkinInvoker(IOperationInvoker originalInvoker)
		{
			_originalInvoker = originalInvoker;
		}

		public bool IsSynchronous => _originalInvoker.IsSynchronous;

		public object[] AllocateInputs() { return _originalInvoker.AllocateInputs(); }

		public object Invoke(object instance, object[] inputs, out object[] outputs)
		{
			var headers = WebOperationContext.Current?.IncomingRequest.Headers;

			var traceIdGenerator = new TraceIdentifierGenerator();
			var sampler = new DebugSampler();

			var traceContext = new TraceContext(traceIdGenerator, sampler)
			{
				TraceId = headers?["X-B3-TraceId"],
				SpanId = headers?["X-B3-SpanId"],
				Debug = headers?["X-B3-Sampled"] == "1"
			};

			var sampled = headers?["X-B3-Sampled"];

			// Sampled should not be set if not flag
			// is provided by the upstream service.
			if (sampled != null)
			{
				traceContext.Sampled = sampled == "1";
			}

			var trace = new ServerTrace(traceContext, "soap");

			trace.Start();

			// Do stuff before call
			var res = _originalInvoker.Invoke(instance, inputs, out outputs);

			trace.End();

			var reporter = new Reporter(new HttpSender("http://localhost:9411"));
			reporter.Report(trace.Span);

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