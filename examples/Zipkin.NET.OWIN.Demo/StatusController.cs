using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zipkin.NET.Dispatchers;
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
            var tracingHandler = new TracingHandler(
                new HttpClientHandler(), 
                new CallContextTraceContextAccessor(), 
                StaticDependencies.Get<Dispatcher>(),
                new RateSampler(1f), 
                "reqres-api");

            var httpClient = new HttpClient(tracingHandler);
            var result = await httpClient.GetAsync(new Uri("https://www.google.com"));
            return Ok(await result.Content.ReadAsStringAsync());
       }
    }
}
