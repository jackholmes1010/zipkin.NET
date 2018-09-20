using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Framework;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.WCF
{
    public class ZipkinBehaviorAttribute : Attribute, IServiceBehavior, IOperationBehavior
    {
        private readonly string _applicationName;

        public ZipkinBehaviorAttribute(string applicationName)
        {
            _applicationName = applicationName;
        }

        // IOperationBehavior
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var traceAccessor = new SystemWebHttpContextTraceAccessor();
            var extractor = new IncomingWebRequestB3Extractor();
            var sampler = new DebugSampler();

            dispatchOperation.Invoker = new ZipkinInvoker(
                _applicationName, dispatchOperation.Invoker, sampler, traceAccessor, extractor);
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        // IServiceBehavior
        public void Validate(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceHostBase.Description.Endpoints)
                foreach (var operation in endpoint.Contract.Operations)
                    operation.Behaviors.Add(this);
        }
    }
}