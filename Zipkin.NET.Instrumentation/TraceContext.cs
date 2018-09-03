namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// Class which contains context information about the the current trace.
    /// </summary>
    public class TraceContext
    {
        private readonly ITraceIdentifierGenerator _traceIdGenerator;

        /// <summary>
        /// Initialize with a custom trace ID generator.
        /// </summary>
        /// <param name="traceIdGenerator">
        /// The custom <see cref="ITraceIdentifierGenerator"/>.
        /// </param>
        public TraceContext(ITraceIdentifierGenerator traceIdGenerator)
        {
            _traceIdGenerator = traceIdGenerator;
        }

        /// <summary>
        /// Initialize with the default <see cref="TraceIdentifierGenerator"/>.
        /// </summary>
        public TraceContext()
        {
            _traceIdGenerator = new TraceIdentifierGenerator();
        }

        /// <summary>
        /// The X-B3-TraceId value associated with the current sever trace.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// The X-B3-SpanId value associated with the current server trace.
        /// </summary>
        public string SpanId { get; set; }

        /// <summary>
        /// The X-B3-ParentSpanId value associated with the current server trace.
        /// </summary>
        public string ParentSpanId { get; set; }

        /// <summary>
        /// The X-B3-Sampled value associated with the current server trace.
        /// </summary>
        public bool Sampled { get; set; }

        /// <summary>
        /// The X-B3-Flags value associated with the current server trace.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Refresh the trace ID's when starting a new trace.
        /// </summary>
        /// <returns>
        /// The same trace with a new span ID and the
        /// parent span ID is equal to the previous span ID.
        /// </returns>
        public TraceContext StartNew()
        {
            TraceId = TraceId ?? _traceIdGenerator.GenerateId();
            ParentSpanId = SpanId ?? _traceIdGenerator.GenerateId();
            SpanId = _traceIdGenerator.GenerateId();
            return this;
        }
    }
}
