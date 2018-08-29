using Newtonsoft.Json;

namespace Zipkin.NET.Instrumentation.Models
{
    /// <summary>
    /// The network context of a node in the service graph.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Endpoint
    {
        /// <summary>
        /// Lower-case label of this node in the service graph.
        /// </summary>
        /// <remarks>
        /// This is a primary label for trace lookup and aggregation, so it should 
        /// be intuitive and consistent.Many use a name from service discovery.
        /// </remarks>
        [JsonProperty("serviceName")]
        public string ServiceName { get; set; }

        /// <summary>
        /// The text representation of the primary IPv4 address associated with this connection.
        /// </summary>
        /// <example>
        /// "192.168.99.100".
        /// </example>
        [JsonProperty("ipv4")]
        public string Ipv4 { get; set; }

        /// <summary>
        /// The text representation of the primary IPv6 address associated with a connection.
        /// </summary>
        /// <example>
        /// "2001:db8::c001".
        /// </example>
        [JsonProperty("ipv6")]
        public string Ipv6 { get; set; }

        /// <summary>
        /// Depending on context, this could be a listen port or the client-side of a socket.
        /// </summary>
        [JsonProperty("port")]
        public int? Port { get; set; }
    }
}
