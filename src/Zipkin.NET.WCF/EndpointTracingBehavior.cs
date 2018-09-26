using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Zipkin.NET.WCF
{
    public abstract class EndpointTracingBehavior : TracingBehaviorBase, IEndpointBehavior
    {
        protected EndpointTracingBehavior(string name) : base(name)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            StartTracer();

            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new TracingMessageInspector(Name));
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            StartTracer();

            clientRuntime.MessageInspectors.Add(new TracingMessageInspector(Name));
        }
    }
}
