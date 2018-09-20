using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;

namespace Zipkin.NET
{
    /// <summary>
    /// Reports completed spans out of process to registered reporters.
    /// </summary>
    public static class TraceManager
    {
        private static readonly List<IReporter> Reporters;
        private static readonly ActionBlock<Span> Processor;

        static TraceManager()
        {
            Reporters = new List<IReporter>();
            Processor = new ActionBlock<Span>(async span => await ReportSpan(span));
        }

        /// <summary>
        /// Asynchronously reports a span using the registered <see cref="IReporter"/>s.
        /// </summary>
        /// <param name="span">
        /// The completed span.
        /// </param>
        public static void Report(Span span)
        {
            Processor.Post(span);
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
                    // TODO log exception
                }
            }
        }
    }
}
