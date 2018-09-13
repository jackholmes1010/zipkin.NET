using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;
using Zipkin.NET.Instrumentation.Traces;

namespace Zipkin.NET.WCF
{
    public class ZipkinInvoker : IOperationInvoker
    {
        private readonly string _applicationName;
        private readonly IOperationInvoker _originalInvoker;
        private readonly IReporter _reporter;
        private readonly ISampler _sampler;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        public ZipkinInvoker(
            string applicationName,
            IOperationInvoker originalInvoker,
            IReporter reporter, 
            ISampler sampler,
            IExtractor<IncomingWebRequestContext> extractor)
        {
            _applicationName = applicationName;
            _originalInvoker = originalInvoker;
            _reporter = reporter;
            _sampler = sampler;
            _extractor = extractor;
        }

        public bool IsSynchronous => _originalInvoker.IsSynchronous;

        public object[] AllocateInputs() { return _originalInvoker.AllocateInputs(); }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var traceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest)
                .NewChildTrace()
                .Sample(_sampler);

            var trace = new ServerTrace(
                traceContext, 
                "soap", 
                local: new Endpoint
                {
                    ServiceName = _applicationName
                });

            trace.Start();

            var response = _originalInvoker.Invoke(instance, inputs, out outputs);

            trace.End();

            _reporter.Report(trace);

            return response;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs,
            AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
            //var res = _originalInvoker.InvokeBegin(instance, inputs, callback, state);
            //return res;
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotImplementedException();
            //var res = _originalInvoker.InvokeEnd(instance, out outputs, result);
            //return res;
        }
    }
}