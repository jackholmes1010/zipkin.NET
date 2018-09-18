using System.Collections.Generic;
using System.Threading.Tasks;
using Zipkin.NET.Models;

namespace Zipkin.NET.Senders
{
    /// <summary>
    /// Sends a list of spans to a transport such as HTTP or Kafka.
    /// </summary>
    /// <remarks>
    /// Usually, this involves encoding them into a message and enqueueing them for 
    /// transport over HTTP or Kafka. The typical end recipient is a zipkin collector.
    /// </remarks>
    public interface ISender
    {
        /// <summary>
        /// Sends a list of spans to a transport such as HTTP or Kafka.
        /// </summary>
        /// <param name="spans">
        /// A list of spans to send.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the async operation.
        /// </returns>
        Task SendSpansAsync(IEnumerable<Span> spans);
    }
}
