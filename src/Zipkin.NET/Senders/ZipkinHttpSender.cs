using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Zipkin.NET.Encoding;
using Zipkin.NET.Models;

namespace Zipkin.NET.Senders
{
    /// <summary>
    /// Sends a collection of spans to the Zipkin span collector endpoint (api/v2/spans).
    /// </summary>
    public class ZipkinHttpSender : ISender
    {
        private readonly string _zipkinHost;
        private readonly IEncoder _encoder;

        /// <summary>
        /// Construct a new <see cref="ZipkinHttpSender"/>.
        /// </summary>
        /// <param name="zipkinHost">
        /// The Zipkin server endpoint.
        /// </param>
        public ZipkinHttpSender(string zipkinHost)
        {
            _zipkinHost = zipkinHost;
            _encoder = new JsonEncoder();
        }

        /// <summary>
        /// Encode the spans as JSON and HTTP POST to the span collector endpoint.
        /// </summary>
        /// <param name="spans">
        /// The spans to send.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/>.
        /// </returns>
        public async Task SendSpansAsync(IEnumerable<Span> spans)
        {
            var client = new HttpClient();
            var encodedSpans = _encoder.Encode(spans);
            var content = new ByteArrayContent(encodedSpans);
            content.Headers.ContentType = new MediaTypeHeaderValue(_encoder.MediaType);
            await client.PostAsync($"{_zipkinHost}/api/v2/spans", content);
        }
    }
}
