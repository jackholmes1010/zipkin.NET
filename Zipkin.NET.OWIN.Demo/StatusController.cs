using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zipkin.NET.Framework;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.OWIN.Demo
{
    public class StatusController : ApiController
    {
        [Route("api/owin/status")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {
            var traceContextAccessor = new CallContextTraceAccessor();
            var propagator = new HttpRequestMessagePropagator();
            var httpClient = new HttpClient(new TracingHandler(
                new HttpClientHandler(), "reqres-api", traceContextAccessor, propagator));
            var result = await httpClient.GetAsync(new Uri("https://www.google.com"));
            return Ok(await result.Content.ReadAsStringAsync());
       }
    }
}
