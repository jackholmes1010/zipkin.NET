using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.WCF
{
    public class ZipkinBehaviorAttribute : Attribute, IServiceBehavior, IOperationBehavior
    {
        private readonly string _applicationName;
        private readonly string _zipkinHost;

        public ZipkinBehaviorAttribute(string applicationName, string zipkinHost)
        {
            _applicationName = applicationName;
            _zipkinHost = zipkinHost;
        }

        // IOperationBehavior
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var reporter = new Reporter(new HttpSender(_zipkinHost));
            var extractor = new IncomingWebRequestB3Extractor();

            dispatchOperation.Invoker = new ZipkinInvoker(
                _applicationName, dispatchOperation.Invoker, reporter, extractor);
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