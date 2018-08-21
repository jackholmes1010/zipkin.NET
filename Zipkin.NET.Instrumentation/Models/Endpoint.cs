namespace Zipkin.Instrumentation.Models
{
    /// <summary>
    /// The network context of a node in the service graph.
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// Lower-case label of this node in the service graph.
        /// </summary>
        /// <remarks>
        /// This is a primary label for trace lookup and aggregation, so it should 
        /// be intuitive and consistent.Many use a name from service discovery.
        /// </remarks>
        public string ServiceName { get; set; }

        /// <summary>
        /// The text representation of the primary IPv4 address associated with this connection.
        /// </summary>
        /// <example>
        /// "192.168.99.100".
        /// </example>
        public string Ipv4 { get; set; }

        /// <summary>
        /// The text representation of the primary IPv6 address associated with a connection.
        /// </summary>
        /// <example>
        /// "2001:db8::c001".
        /// </example>
        public string Ipv6 { get; set; }

        /// <summary>
        /// Depending on context, this could be a listen port or the client-side of a socket.
        /// </summary>
        public int? Port { get; set; }
    }
}
