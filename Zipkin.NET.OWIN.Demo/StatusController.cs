using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Zipkin.NET.Framework;
using Zipkin.NET.Reporters;
using Zipkin.NET.Senders;

namespace Zipkin.NET.OWIN.Demo
{
    public class StatusController : ApiController
    {
        [Route("api/owin/status")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {
            var reporter = new Reporter(new HttpSender("http://localhost:9411"));
            var traceContextAccessor = new CallContextTraceAccessor();
            var propagator = new HttpRequestMessagePropagator();
            var httpClient = new HttpClient(new TracingHandler(
                new HttpClientHandler(), "reqres-api", reporter, traceContextAccessor, propagator));
            var result = await httpClient.GetAsync(new Uri("https://www.google.com"));
            return Ok(await result.Content.ReadAsStringAsync());
       }
    }
}
