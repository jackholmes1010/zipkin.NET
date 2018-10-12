using System.Collections.Generic;
using Zipkin.NET.Models;

namespace Zipkin.NET.Encoding
{
    /// <summary>
    /// Encode spans as byte arrays.
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// The MIME type.
        /// </summary>
        string MediaType { get; }

        /// <summary>
        /// Encode spans as a byte array.
        /// </summary>
        /// <param name="spans">
        /// The spans to encode.
        /// </param>
        /// <returns>
        /// The encoded spans.
        /// </returns>
        byte[] Encode(IEnumerable<Span> spans);
    }
}
