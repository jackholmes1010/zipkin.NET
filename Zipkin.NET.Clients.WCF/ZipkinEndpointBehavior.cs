using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;
namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Endpoint behavior responsible for applying the <see cref="ZipkinMessageInspector"/> to the client runtime.
    /// </summary>
    /// <inheritdoc />
    public class ZipkinEndpointBehavior : IEndpointBehavior
    {
        private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ITraceContextAccessor _traceContextAccessor;

        public ZipkinEndpointBehavior(
            string applicationName,
            IReporter reporter, 
            ITraceContextAccessor traceContextAccessor)
        {
            _applicationName = applicationName;
            _reporter = reporter;
            _traceContextAccessor = traceContextAccessor;
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
            clientRuntime.ClientMessageInspectors.Add(
                new ZipkinMessageInspector(_applicationName, _reporter, _traceContextAccessor));
        }
    }
}
