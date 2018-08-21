using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zipkin.NET.Instrumentation.Reporting
{
    public class HttpSender : ISender
    {
        public async Task SendSpansAsync(IEnumerable<byte[]> encodedSpans)
        {
            // TODO this is a test
            var client = new HttpClient();

            foreach (var span in encodedSpans)
            {
                var content = new ByteArrayContent(span);
                content.Headers.Add("Content-Type", "application/json");
                content.Headers.Add("Content-Length", span.Length.ToString());
                await client.PostAsync("http://localhost:9411", content);
            }
        }
    }
}
