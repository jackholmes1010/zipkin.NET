using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Zipkin.NET.Instrumentation.Converters;

namespace Zipkin.NET.Instrumentation.Models
{
    /// <summary>
    /// A span is a single-host view of an operation.
    /// </summary>
    /// <remarks>
    /// A trace is a series of spans (often RPC calls) which nest to form a latency tree. 
    /// Spans are in the same a trace when they share the same trace ID. 
    /// The ParentId field establishes the position of one span in the tree.
    /// </remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public class Span
    {
        /// <summary>
        /// Unique 64-bit identifier for this operation within the trace.
        /// </summary>
        /// <remarks>
        /// Encoded as 16 lowercase hex characters.
        /// </remarks>
        /// <example>
        /// "ffdc9bb9a6453df3"
        /// </example>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Randomly generated, unique identifier for a trace, set on all spans within it.
        /// </summary>
        /// <remarks>
        /// Encoded as 16 or 32 lowercase hex characters corresponding to 64 or 128 bits.
        /// </remarks>
        /// <example>
        /// A 128-bit trace ID looks like "4e441824ec2b6a44ffdc9bb9a6453df3".
        /// </example>
        [JsonProperty("traceId")]
        public string TraceId { get; set; }

        /// <summary>
        /// The parent span ID or absent if this the root span in a trace.
        /// </summary>
        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        /// <summary>
        /// When present, kind clarifies timestamp, duration and remoteEndpoint. 
        /// </summary>
        /// <remarks>
        /// When absent, the span is local or incomplete. Unlike client and server, there is
        /// no direct critical path latency relationship between producer and consumer spans.
        /// </remarks>
        [JsonProperty("kind")]
        public SpanKind? Kind { get; set; }

        /// <summary>
        /// The logical operation this span represents in lowercase (e.g. rpc method).
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Epoch microseconds of the start of this span.
        /// </summary>
        /// <example>
        /// 1502787600000000 corresponds to 2017-08-15 09:00 UTC.
        /// </example>
        [JsonConverter(typeof(UnixTimeStampDateTimeConverter))]
        [JsonProperty("timestamp")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Duration in microseconds of the critical path, if known. 
        /// </summary>
        /// <remarks>
        /// Durations of less than one are rounded up. Duration of children 
        /// can be longer than their parents due to asynchronous operations.
        /// </remarks>
        /// <example>
        /// 150 milliseconds is 150000 microseconds.
        /// </example>
        [JsonConverter(typeof(TimeSpanConverter))]
        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// True is a request to store this span even if it overrides sampling policy.
        /// </summary>
        /// <remarks>
        /// This is true when the X-B3-Flags header has a value of 1.
        /// </remarks>
        [JsonProperty("false")]
        public bool Debug { get; set; }

        /// <summary>
        /// True if we are contributing to a span started by another tracer (ex on a different host).
        /// </summary>
        [JsonProperty("shared")]
        public bool Shared { get; set; }

        /// <summary>
        /// The host that recorded this span, primarily for query by service name.
        /// </summary>
        [JsonProperty("localEndpoint")]
        public Endpoint LocalEndpoint { get; set; }

        /// <summary>
        /// When an RPC (or messaging) span, indicates the other side of the connection.
        /// </summary>
        [JsonProperty("remoteEndpoint")]
        public Endpoint RemoteEndpoint { get; set; }

        /// <summary>
        /// Event information associated with this span.
        /// </summary>
        [JsonProperty("annotations")]
        public IList<Annotation> Annotations { get; set; }

        /// <summary>
        /// Adds context to a span, for search, viewing and analysis.
        /// </summary>
        [JsonProperty("tags")]
        public IDictionary<string, string> Tags { get; set; }
    }
}
