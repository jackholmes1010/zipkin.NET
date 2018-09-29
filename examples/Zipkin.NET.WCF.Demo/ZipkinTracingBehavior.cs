using System;
using System.Collections.Generic;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;

namespace Zipkin.NET.WCF.Demo
{
    public class ZipkinTracingBehavior : ServiceTracingBehavior
    {
        public ZipkinTracingBehavior(string name) : base(name)
        {
        }

        protected override Sampler Sampler => new RateSampler(1f);

        protected override Dispatcher Dispatcher => new AsyncActionBlockDispatcher(Reporters, Logger, TraceContextAccessor);

        protected override ITraceContextAccessor TraceContextAccessor => new CallContextTraceContextAccessor();

        protected override IInstrumentationLogger Logger => new ConsoleInstrumentationLogger();

        protected override IEnumerable<IReporter> Reporters
        {
            get
            {
                var sender = new ZipkinHttpSender("http://localhost:9411");
                var reporter = new ZipkinReporter(sender);
                return new[] { reporter };
            }
        }

        public override Type BehaviorType => typeof(ZipkinTracingBehavior);

        protected override object CreateBehavior()
        {
            return new ZipkinTracingBehavior(Name);
        }

    }
}