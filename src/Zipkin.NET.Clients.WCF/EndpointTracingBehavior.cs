using System;
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
        private readonly Func<ITraceContextAccessor> _traceContextAccessor;
        private readonly Func<ISampler> _sampler;
        private readonly Func<IDispatcher> _dispatcher;

        public EndpointTracingBehavior(
            string remoteEndpointName,
            Func<ITraceContextAccessor> traceContextAccessor,
            Func<ISampler> sampler,
            Func<IDispatcher> dispatcher)
        {
            _remoteEndpointName = remoteEndpointName;
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
                new ClientTracingMessageInspector(_remoteEndpointName, _traceContextAccessor, _sampler, _dispatcher));
        }
    }
}
