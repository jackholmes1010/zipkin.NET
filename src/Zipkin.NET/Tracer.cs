using System;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Logging;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    /// <summary>
    /// Used by instrumentation to access singleton dependencies.
    /// </summary>
    public static class Tracer
    {
        private static ITraceContextAccessor _contextAccessor;
        private static Sampler _sampler;
        private static IInstrumentationLogger _logger;
        private static Dispatcher _dispatcher;

        /// <summary>
        /// Register standard dependencies required to start the tracer.
        /// </summary>
        /// <param name="sampler">
        /// A <see cref="Sampler"/> used to make sampling decisions.
        /// </param>
        /// <param name="dispatcher">
        /// A <see cref="Dispatchers.Dispatcher"/> used to dispatch completed spans.
        /// </param>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context
        /// across for the current request context, across the application.
        /// </param>
        /// <param name="logger">
        /// A <see cref="IInstrumentationLogger"/> used by instrumentation to log errors.
        /// </param>
        public static void Start(
            Sampler sampler,
            Dispatcher dispatcher,
            ITraceContextAccessor traceContextAccessor,
            IInstrumentationLogger logger)
        {
            Sampler = sampler;
            Dispatcher = dispatcher;
            ContextAccessor = traceContextAccessor;
            Logger = logger;
            Started = true;
        }

        /// <summary>
        /// Stop the tracer by removing all dependencies and setting the Start property to false.
        /// </summary>
        public static void Stop()
        {
            Sampler = null;
            Dispatcher = null;
            ContextAccessor = null;
            Logger = null;
            Started = false;
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
                        "ContextAccessor is null. Make sure the Tracer has been started by calling Tracer.Start() in application startup.");
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
                        "Sampler is null. Make sure the Tracer has been started by calling Tracer.Start() in application startup.");
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
                        "Logger is null. Make sure the Tracer has been started by calling Tracer.Start() in application startup.");
                }

                return _logger;
            }

            private set => _logger = value;
        }

        /// <summary>
        /// Gets a <see cref="IInstrumentationLogger"/> used by instrumentation to log errors.
        /// </summary>
        public static Dispatcher Dispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    throw new Exception(
                        "Logger is null. Make sure the Tracer has been started by calling Tracer.Start() in application startup.");
                }

                return _dispatcher;
            }

            private set => _dispatcher = value;
        }
    }
}
