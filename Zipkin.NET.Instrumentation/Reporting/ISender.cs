using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zipkin.NET.Instrumentation.Reporting
{
    /// <summary>
    /// Sends a list of encoded spans to a transport such as HTTP or Kafka. 
    /// </summary>
    /// <remarks>
    /// Usually, this involves encoding them into a message and enqueueing them for 
    /// transport over http or Kafka. The typical end recipient is a zipkin collector.
    /// </remarks>
    public interface ISender
    {
        /// <summary>
        /// Sends a list of encoded spans to a transport such as HTTP or Kafka.
        /// </summary>
        /// <param name="encodedSpans">
        /// A list of encoded spans to send.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the async operation.
        /// </returns>
        Task SendSpansAsync(IEnumerable<byte[]> encodedSpans);
    }
}
