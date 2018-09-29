using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.WCF.MessageInspectors;

namespace Zipkin.NET.WCF.Behaviors
{
    /// <summary>
    /// An <see cref="IEndpointBehavior"/> which adds a <see cref="DispatchTracingMessageInspector"/> to a WCF client.
    /// <remarks>
    /// Override this to build and report spans from WCF client requests.
    /// </remarks>
    /// </summary>
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
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
                new DispatchTracingMessageInspector(
                    Name,
                    TraceContextAccessor,
                    Sampler,
                    Dispatcher));
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(
                new ClientTracingMessageInspector(
                    Name,
                    TraceContextAccessor,
                    Sampler,
                    Dispatcher));
        }
    }
}
