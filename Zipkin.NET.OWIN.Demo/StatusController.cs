using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.OWIN.Demo
{
    public class StatusController : ApiController
    {
        [Route("api/owin/status")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {
			var reporter = new Reporter(new HttpSender("http://localhost:9411"));
			var sampler = new DebugSampler();
			var traceContextAccessor = new CallContextTraceContextAccessor();
			var propagator = new HttpRequestMessageB3Propagator();
            var httpClient = new HttpClient(new ZipkinHandler(
				new HttpClientHandler(), "reqres-api", reporter, sampler, traceContextAccessor, propagator));
            var result = await httpClient.GetAsync(new Uri("https://reqres.in/api/users?page=2"));
            return Ok(await result.Content.ReadAsStringAsync());
        }
    }
}
