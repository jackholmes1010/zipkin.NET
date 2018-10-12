using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zipkin.NET.Models;

namespace Zipkin.NET.Encoding
{
    /// <summary>
    /// Encode spans as JSON.
    /// </summary>
    public class JsonEncoder : IEncoder
    {
        /// <summary>
        /// application/json MIME type.
        /// </summary>
        public string MediaType => "application/json";

        /// <summary>
        /// Serializes a list of spans as JSON and returns a sequence of bytes.
        /// </summary>
        /// <param name="spans">
        /// The spans to encode.
        /// </param>
        /// <returns>
        /// The JSON encoded spans as a sequence of bytes.
        /// </returns>
        public byte[] Encode(IEnumerable<Span> spans)
        {
            var serializedSpans = JsonConvert.SerializeObject(
                spans, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            return System.Text.Encoding.UTF8.GetBytes(serializedSpans);
        }
    }
}
