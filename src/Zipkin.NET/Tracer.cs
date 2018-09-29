using System;
using System.Collections.Generic;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    /// <summary>
    /// Reports completed spans out of process to registered reporters.
    /// </summary>
    public static class Tracer
    {
        private static ITraceContextAccessor _contextAccessor;
        private static Sampler _sampler;
        private static IInstrumentationLogger _logger;
        private static IDispatcher _dispatcher;

        /// <summary>
        /// Register standard dependencies required to start the tracer.
        /// </summary>
        /// <param name="sampler">
        /// A <see cref="Sampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="dispatcher">
        /// A <see cref="IDispatcher"/> used to dispatch completed spans.
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
            IDispatcher dispatcher,
            ITraceContextAccessor traceContextAccessor,
            IInstrumentationLogger logger,
            IEnumerable<IReporter> reporters)
        {
            Sampler = sampler;
            Dispatcher = dispatcher;
            ContextAccessor = traceContextAccessor;
            Logger = logger;
            Started = true;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the tracer has been started.
        /// </summary>
        public static bool Started { get; private set; }

        /// <summary>
        /// Gets a <see cref="ITraceContextAccessor"/> used to access trace context for the current request.
        /// </summary>
        public static ITraceContextAccessor ContextAccessor
        {
            get
            {
                if (_contextAccessor == null)
                {
                    throw new Exception(
                        "ContextAccessor is null. Make sure the Tracer has been started by calling Tracer.Start().");
                }

                return _contextAccessor;
            }

            private set => _contextAccessor = value;
        }

        /// <summary>
        /// Gets a <see cref="Sampler"/> used to make sampling decisions about traces.
        /// </summary>
        public static Sampler Sampler
        {
            get
            {
                if (_sampler == null)
                {
                    throw new Exception(
                        "Sampler is null. Make sure the Tracer has been started by calling Tracer.Start().");
                }

                return _sampler;
            }

            private set => _sampler = value;
        }

        /// <summary>
        /// Gets a <see cref="IInstrumentationLogger"/> used by instrumentation to log errors.
        /// </summary>
        public static IInstrumentationLogger Logger
        {
            get
            {
                if (_sampler == null)
                {
                    throw new Exception(
                        "Logger is null. Make sure the Tracer has been started by calling Tracer.Start().");
                }

                return _logger;
            }

            private set => _logger = value;
        }

        /// <summary>
        /// Gets a <see cref="IInstrumentationLogger"/> used by instrumentation to log errors.
        /// </summary>
        public static IDispatcher Dispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    throw new Exception(
                        "Logger is null. Make sure the Tracer has been started by calling Tracer.Start().");
                }

                return _dispatcher;
            }

            private set => _dispatcher = value;
        }
    }
}
