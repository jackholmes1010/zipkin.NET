using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.WCF
{
    public abstract class TracingBehavior : BehaviorExtensionElement, IEndpointBehavior, IServiceBehavior
    {
        protected abstract Sampler Sampler { get; }

        protected abstract ITraceContextAccessor TraceContextAccessor { get; }

        protected abstract IEnumerable<IReporter> Reporters { get; }

        protected abstract IInstrumentationLogger Logger { get; }

        public override Type BehaviorType => typeof(TracingBehavior);

        protected abstract override object CreateBehavior();

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            StartTracer();

            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new TracingMessageInspector("test"));
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            StartTracer();

            clientRuntime.MessageInspectors.Add(new TracingMessageInspector("application-name"));
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
            StartTracer();

            foreach(var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = (ChannelDispatcher) channelDispatcherBase;
                foreach(var endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new TracingMessageInspector("test"));
                }
            }
        }

        private void StartTracer()
        {
            if (!Tracer.Started)
            {
                Tracer.Start(Sampler, TraceContextAccessor, Logger, Reporters);
            }
        }
    }
}