using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Endpoint behavior responsible for applying the <see cref="ZipkinMessageInspector"/> to the client runtime.
    /// </summary>
    /// <inheritdoc />
    public class ZipkinEndpointBehavior : IEndpointBehavior
    {
        private readonly string _applicationName;
        private readonly ITraceAccessor _traceAccessor;

        public ZipkinEndpointBehavior(
            string applicationName,
            ITraceAccessor traceAccessor)
        {
            _applicationName = applicationName;
            _traceAccessor = traceAccessor;
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
                new ZipkinMessageInspector(_applicationName, _traceAccessor, propagator));
        }
    }
}
