using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.WCF.MessageInspectors;

namespace Zipkin.NET.WCF.Behaviors
{
    /// <summary>
    /// An <see cref="IServiceBehavior"/> which adds a <see cref="DispatchTracingMessageInspector"/> to a service endpoint.
    /// <remarks>
    /// Override this to build and report spans from requests entering a WCF service.
    /// </remarks>
    /// </summary>
    public class ServiceTracingBehavior : IServiceBehavior
    {
        private readonly string _localEndpointName;
        private readonly float _sampleRate;
        private readonly string _zipkinHost;

        public ServiceTracingBehavior(string localEndpointName, string zipkinHost, float sampleRate)
        {
            _localEndpointName = localEndpointName;
            _zipkinHost = zipkinHost;
            _sampleRate = sampleRate;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = (ChannelDispatcher)channelDispatcherBase;
                foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
                        new DispatchTracingMessageInspector(_localEndpointName, _zipkinHost, _sampleRate));
                }
            }
        }
    }
}
