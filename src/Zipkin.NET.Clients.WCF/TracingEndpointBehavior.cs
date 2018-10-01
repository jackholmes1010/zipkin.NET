using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Endpoint behavior responsible for adding a <see cref="TracingMessageInspector"/> to the client runtime.
    /// </summary>
    public class TracingEndpointBehavior : IEndpointBehavior
    {
        private readonly string _applicationName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly ISampler _sampler;
        private readonly IDispatcher _dispatcher;

        public TracingEndpointBehavior(
            string applicationName,
            ITraceContextAccessor traceContextAccessor,
            ISampler sampler,
            IDispatcher dispatcher)
        {
            _applicationName = applicationName;
            _traceContextAccessor = traceContextAccessor;
            _sampler = sampler;
            _dispatcher = dispatcher;
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
                new TracingMessageInspector(_applicationName, _traceContextAccessor, _sampler, _dispatcher));
        }
    }
}
