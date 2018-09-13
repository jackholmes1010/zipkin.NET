using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.WCF
{
    public class ZipkinInvoker : IOperationInvoker
    {
        private readonly string _applicationName;
        private readonly IOperationInvoker _originalInvoker;
        private readonly IReporter _reporter;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        public ZipkinInvoker(
            string applicationName,
            IOperationInvoker originalInvoker,
            IReporter reporter, 
            IExtractor<IncomingWebRequestContext> extractor)
        {
            _applicationName = applicationName;
            _originalInvoker = originalInvoker;
            _reporter = reporter;
            _extractor = extractor;
        }

        public bool IsSynchronous => _originalInvoker.IsSynchronous;

        public object[] AllocateInputs() { return _originalInvoker.AllocateInputs(); }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var traceContext = _extractor.Extract(
                WebOperationContext.Current?.IncomingRequest);

            var trace = new ServerTrace(
                new DebugSampler(),
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
            _reporter.Report(trace);

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