using System;
using System.Text;

namespace Zipkin.NET.Instrumentation
{
    /// <summary>
    /// Class which contains context information about the the current trace.
    /// </summary>
    public class TraceContext
    {
        /// <summary>
        /// The trace ID value associated with the current trace.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// The span ID value associated with the current trace.
        /// </summary>
        public string SpanId { get; set; }

        /// <summary>
        /// The parent span ID associated with the current trace.
        /// </summary>
        public string ParentSpanId { get; set; }

        /// <summary>
        /// The sampled value associated with the current trace.
        /// </summary>
        public bool? Sampled { get; set; }

        /// <summary>
        /// The debug values associated with the current trace.
        /// </summary>
        public bool? Debug { get; set; }

	    /// <summary>
	    /// Refresh the trace ID's when starting a new child trace.
	    /// </summary>
	    /// <returns>
	    /// A new <see cref="TraceContext"/> instance with a new
	    /// span ID and parent span ID equal to the previous span ID.
	    /// </returns>
	    public TraceContext NewChildTrace()
	    {
		    var random = new Random();

		    return new TraceContext
		    {
			    TraceId = TraceId ?? GenerateTraceId(random),
			    ParentSpanId = SpanId ?? GenerateTraceId(random),
			    SpanId = GenerateTraceId(random),
				Debug = Debug,
				Sampled = Sampled
			};
	    }

	    /// <summary>
	    /// Generate a 64-bit trace ID.
	    /// </summary>
	    /// <returns>
	    /// The trace ID as a string.
	    /// </returns>
	    public virtual string GenerateTraceId(Random random = null)
	    {
		    // TODO this is stupid
		    if (random == null)
				random = new Random();

		    var builder = new StringBuilder();
		    for (var i = 0; i < 16; i++)
		    {
			    builder.Append(random.Next(0, 15).ToString("X").ToLower());
		    }

		    return builder.ToString();

		    //      var bytes = new byte[8];
		    //      var cryptoProvider = new RNGCryptoServiceProvider();
		    //cryptoProvider.GetBytes(bytes);
		    //      var id = BitConverter.ToString(bytes);
		    //      return id.Replace("-", string.Empty);
	    }
	}
}
