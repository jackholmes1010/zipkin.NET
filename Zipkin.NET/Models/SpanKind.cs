using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zipkin.NET.Models
{
    /// <summary>
    /// The span kind is used to clarify a span's timestamp, duration, and remote endpoint.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpanKind
    {
        /// <summary>
        /// <para>
        /// Span timestamp is the moment a request was sent to the server (in v1 "cs").
        /// </para>
        /// <para>
        /// Span duration is the delay until a response or an error was received (in v1 "cr"-"cs").
        /// </para>
        /// <para>
        /// Span remote endpoint is the server (in v1 "sa").
        /// </para>
        /// </summary>
        [EnumMember(Value = "CLIENT")]
        Client,

        /// <summary>
        /// <para>
        /// Span timestamp is the moment a client request was received (in v1 "sr").
        /// </para>
        /// <para>
        /// Span duration is the delay until a response was sent or an error (in v1 "ss"-"sr").
        /// </para>
        /// <para>
        /// Span remote endpoint is the client (in v1 "ca").
        /// </para>
        /// </summary>
        [EnumMember(Value = "SERVER")]
        Server,

        /// <summary>
        /// <para>
        /// Span timestamp is the moment a message was sent to a destination (in v1 "ms").
        /// </para>
        /// <para>
        /// Span duration is the delay sending the message, such as batching.
        /// </para>
        /// <para>
        /// Span remote endpoint is the broker.
        /// </para>
        /// </summary>
        [EnumMember(Value = "PRODUCER")]
        Producer,

        /// <summary>
        /// <para>
        /// Span timestamp is the moment a message was received from an origin (in v1 "mr").
        /// </para>
        /// <para>
        /// Span duration is the delay consuming the message, such as from backlog.
        /// </para>
        /// <para>
        /// Span remote endpoint - Represents the broker. Leave serviceName absent if unknown.
        /// </para>
        /// </summary>
        [EnumMember(Value = "CONSUMER")]
        Consumer
    }
}
