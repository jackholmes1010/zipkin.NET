using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation.Reporting
{
    public class HttpSender : ISender
    {
        public async Task SendSpansAsync(IEnumerable<Span> spans)
        {
            var client = new HttpClient();
            var serializedSpans = JsonConvert.SerializeObject(
                spans, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var content = new StringContent(serializedSpans);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync("http://localhost:9411/api/v2/spans", content);
            await result.Content.ReadAsStringAsync();
        }
    }
}
