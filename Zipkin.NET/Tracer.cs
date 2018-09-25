using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Zipkin.NET.Logging;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    /// <summary>
    /// Reports completed spans out of process to registered reporters.
    /// </summary>
    public static class Tracer
    {
        private static readonly List<IReporter> Reporters;
        private static readonly ActionBlock<Span> Processor;

        static Tracer()
        {
            Reporters = new List<IReporter>();
            Processor = new ActionBlock<Span>(async span => await ReportSpan(span));
        }

        /// <summary>
        /// Register standard dependencies required to start the tracer.
        /// </summary>
        /// <param name="sampler">
        /// A <see cref="Sampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context
        /// across for the current request context, across the application.
        /// </param>
        /// <param name="logger">
        /// A <see cref="IInstrumentationLogger"/> used by instrumentation to log errors.
        /// </param>
        /// <param name="reporters">
        /// A collection of <see cref="IReporter"/>s used to report spans recorded by instrumentation.
        /// </param>
        public static void Start(
            Sampler sampler,
            ITraceContextAccessor traceContextAccessor,
            IInstrumentationLogger logger,
            IEnumerable<IReporter> reporters)
        {
            Sampler = sampler;
            ContextAccessor = traceContextAccessor;
            Logger = logger;

            foreach (var reporter in reporters)
            {
                var exists = Reporters.Any(r => r.GetHashCode() == reporter.GetHashCode());
                if (!exists)
                {
                    Reporters.Add(reporter);
                }
            }

            Started = true;
        }

        /// <summary>
        /// Gets a <see cref="ITraceContextAccessor"/> used to access trace context for the current request.
        /// </summary>
        public static ITraceContextAccessor ContextAccessor { get; private set; }

        /// <summary>
        /// Gets a <see cref="Sampler"/> used to make sampling decisions about traces.
        /// </summary>
        public static Sampler Sampler { get; private set; }

        /// <summary>
        /// Gets a <see cref="IInstrumentationLogger"/> used by instrumentation to log errors.
        /// </summary>
        public static IInstrumentationLogger Logger { get; private set; }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the tracer has been started.
        /// </summary>
        public static bool Started { get; private set; }

        /// <summary>
        /// Asynchronously reports a span using the registered <see cref="IReporter"/>s.
        /// </summary>
        /// <param name="traceContext">
        /// The current <see cref="TraceContext"/>.
        /// </param>
        /// <param name="span">
        /// The completed span.
        /// </param>
        public static void Report(TraceContext traceContext, Span span)
        {
            if (!Started)
            {
                throw new Exception("Tracer has not been started. Call Start() to start tracer.");
            }

            if (traceContext.Sampled == true)
            {
                Processor.Post(span);
            }
        }

        private static async Task ReportSpan(Span span)
        {
            foreach (var reporter in Reporters)
            {
                try
                {
                    await reporter.ReportAsync(span);
                }
                catch (Exception ex)
                {
                    Logger.WriteError(ex.ToString());
                }
            }
        }
    }
}
