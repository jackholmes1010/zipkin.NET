using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Endpoint behavior responsible for adding a <see cref="ClientTracingMessageInspector"/> to the client runtime.
    /// </summary>
    public class EndpointTracingBehavior : IEndpointBehavior
    {
        private readonly string _remoteEndpointName;
        private readonly ISampler _sampler;
        private readonly IDispatcher _dispatcher;
        private readonly ITraceContextAccessor _traceContextAccessor;

        public EndpointTracingBehavior(
            string remoteEndpointName, 
            ISampler sampler, 
            IDispatcher dispatcher,
            ITraceContextAccessor traceContextAccessor)
        {
            _remoteEndpointName = remoteEndpointName;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _traceContextAccessor = traceContextAccessor;
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
            clientRuntime.ClientMessageInspectors.Add(
                new ClientTracingMessageInspector(_remoteEndpointName, _sampler, _dispatcher, _traceContextAccessor));
        }
    }
}
