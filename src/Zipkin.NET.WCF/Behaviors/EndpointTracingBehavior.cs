using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Sampling;
using Zipkin.NET.WCF.MessageInspectors;

namespace Zipkin.NET.WCF.Behaviors
{
    public class EndpointTracingBehavior : IEndpointBehavior
    {
        private readonly string _remoteEndpointName;
        private readonly ISampler _sampler;
        private readonly IDispatcher _dispatcher;
        private readonly ISpanContextAccessor _spanContextAccessor;

        public EndpointTracingBehavior(
            string remoteEndpointName,
            ISampler sampler,
            IDispatcher dispatcher,
            ISpanContextAccessor spanContextAccessor)
        {
            _remoteEndpointName = remoteEndpointName;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _spanContextAccessor = spanContextAccessor;
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
                new ClientTracingMessageInspector(_remoteEndpointName, _sampler, _dispatcher, _spanContextAccessor));
        }
    }
}
