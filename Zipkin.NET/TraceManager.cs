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
    public static class TraceManager
    {
        private static readonly List<IReporter> Reporters;
        private static readonly List<IInstrumentationLogger> Loggers;
        private static readonly ActionBlock<Span> Processor;
        private static Sampler _sampler;

        static TraceManager()
        {
            Reporters = new List<IReporter>();
            Loggers = new List<IInstrumentationLogger>();
            Processor = new ActionBlock<Span>(async span => await ReportSpan(span));
        }

        /// <summary>
        /// Register a <see cref="Sampler"/> used to make sampling decisions.
        /// </summary>
        /// <param name="sampler">
        /// The <see cref="Sampler"/>.
        /// </param>
        public static void RegisterSampler(Sampler sampler)
        {
            _sampler = sampler;
        }

        /// <summary>
        /// Register a custom <see cref="IInstrumentationLogger"/>.
        /// <remarks>
        /// By default, a <see cref="ConsoleInstrumentationLogger"/> is used.
        /// </remarks>
        /// </summary>
        /// <param name="logger">
        /// The <see cref="IInstrumentationLogger"/>.
        /// </param>
        public static void RegisterLogger(IInstrumentationLogger logger)
        {
            var exists = Loggers.Any(l => l.GetType() == logger.GetType());
            if (!exists)
            {
                Loggers.Add(logger);
            }
        }

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
            if (traceContext.Sampled == true)
            {
                Processor.Post(span);
            }
        }

        /// <summary>
        /// Register a reporter to receive completed spans.
        /// </summary>
        /// <param name="reporter">
        /// The <see cref="IReporter"/> to register.
        /// </param>
        public static void Register(IReporter reporter)
        {
            Reporters.Add(reporter);
        }

        /// <summary>
        /// Make a sampling decision using the registered <see cref="Sampler"/>.
        /// </summary>
        /// <param name="traceContext">
        /// The current <see cref="TraceContext"/>.
        /// </param>
        public static void Sample(ref TraceContext traceContext)
        {
            if (_sampler == null)
            {
                foreach (var logger in Loggers)
                {
                    logger.WriteError("No sampler is registered.");
                }
            }
            else
            {
                _sampler.IsSampled(ref traceContext);
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
                    foreach (var logger in Loggers)
                    {
                        logger.WriteError(ex.ToString());
                    }
                }
            }
        }
    }
}
