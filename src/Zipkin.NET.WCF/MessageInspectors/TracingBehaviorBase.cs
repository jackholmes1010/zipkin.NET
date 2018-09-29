using System.Collections.Generic;
using System.ServiceModel.Configuration;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.WCF.MessageInspectors
{
    public abstract class TracingBehaviorBase : BehaviorExtensionElement
    {
        protected readonly string Name;

        protected TracingBehaviorBase(string name)
        {
            Name = name;
        }

        protected abstract Sampler Sampler { get; }

        protected abstract ITraceContextAccessor TraceContextAccessor { get; }

        protected abstract IEnumerable<IReporter> Reporters { get; }

        protected abstract IInstrumentationLogger Logger { get; }

        protected abstract Dispatcher Dispatcher { get; }
    }
}
