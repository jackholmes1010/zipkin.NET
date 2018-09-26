using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Endpoint behavior responsible for applying the <see cref="TracingMessageInspector"/> to the client runtime.
    /// </summary>
    /// <inheritdoc />
    public class TracingEndpointBehavior : IEndpointBehavior
    {
        private readonly string _applicationName;
        private readonly ITraceContextAccessor _traceContextAccessor;

        public TracingEndpointBehavior(
            string applicationName,
            ITraceContextAccessor traceContextAccessor)
        {
            _applicationName = applicationName;
            _traceContextAccessor = traceContextAccessor ?? throw new ArgumentNullException(nameof(traceContextAccessor));
        }

        /// <inheritdoc />
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        /// <inheritdoc />
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <inheritdoc />
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        /// <inheritdoc />
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var propagator = new HttpRequestMessagePropertyB3Propagator();

            clientRuntime.ClientMessageInspectors.Add(
                new TracingMessageInspector(_applicationName, _traceContextAccessor, propagator));
        }
    }
}
