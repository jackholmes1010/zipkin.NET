using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Zipkin.NET.WCF.Demo
{
	public class ZipkinBehaviorAttribute : Attribute, IServiceBehavior, IOperationBehavior
	{
		// IOperationBehavior
		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.Invoker = new ZipkinInvoker(dispatchOperation.Invoker);
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