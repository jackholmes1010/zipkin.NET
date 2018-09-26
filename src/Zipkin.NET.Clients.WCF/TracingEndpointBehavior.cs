using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Endpoint behavior responsible for applying the <see cref="TracingMessageInspector"/> to the client runtime.
    /// </summary>
    public class TracingEndpointBehavior : IEndpointBehavior
    {
        private readonly string _applicationName;

        public TracingEndpointBehavior(string applicationName)
        {
            _applicationName = applicationName;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new TracingMessageInspector(_applicationName));
        }
    }
}
