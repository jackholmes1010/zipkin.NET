using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Endpoint behavior responsible for applying the <see cref="TracingMessageInspector"/> to the client runtime.
    /// </summary>
    /// <inheritdoc />
    public class TracingEndpointBehavior : IEndpointBehavior
    {
        private readonly string _applicationName;
        private readonly ITraceAccessor _traceAccessor;

        public TracingEndpointBehavior(
            string applicationName,
            ITraceAccessor traceAccessor)
        {
            _applicationName = applicationName;
            _traceAccessor = traceAccessor ?? throw new ArgumentNullException(nameof(traceAccessor));
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
                new TracingMessageInspector(_applicationName, _traceAccessor, propagator));
        }
    }
}
