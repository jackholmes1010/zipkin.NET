using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;

namespace Zipkin.NET.WCF.Demo
{
    public class ZipkinTracingBehavior : TracingBehavior
    {
        protected override Sampler Sampler => new DebugSampler();

        protected override ITraceContextAccessor TraceContextAccessor => new CallContextTraceContextAccessor();

        protected override IEnumerable<IReporter> Reporters
        {
            get
            {
                var sender = new HttpSender("http://localhost:9411");
                var reporter = new ZipkinReporter(sender);
                return new[] {reporter};
            }
        }

        protected override IInstrumentationLogger Logger => new ConsoleInstrumentationLogger();

        protected override object CreateBehavior()
        {
            return new ZipkinTracingBehavior();
        }
    }
}