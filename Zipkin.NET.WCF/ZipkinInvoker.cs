using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET.WCF
{
    public class ZipkinInvoker : IOperationInvoker
    {
        private readonly string _applicationName;
        private readonly IOperationInvoker _originalInvoker;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        public ZipkinInvoker(
            string applicationName,
            IOperationInvoker originalInvoker)
        {


            _applicationName = applicationName;
            _originalInvoker = originalInvoker ?? throw new ArgumentNullException(nameof(originalInvoker));
            _extractor = new IncomingWebRequestB3Extractor();
        }

        public bool IsSynchronous => _originalInvoker.IsSynchronous;

        public object[] AllocateInputs() { return _originalInvoker.AllocateInputs(); }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var traceContext = _extractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            Tracer.Sampler.Sample(ref traceContext);

            var spanBuilder = traceContext
                .GetSpanBuilder()
                .Start()
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                });
                
            Tracer.ContextAccessor.SaveTrace(traceContext);

            var response = _originalInvoker.Invoke(instance, inputs, out outputs);

            spanBuilder.End();

            Tracer.Report(traceContext, spanBuilder.Build());

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