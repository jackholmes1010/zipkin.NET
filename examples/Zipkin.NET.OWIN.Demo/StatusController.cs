using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.OWIN.Demo
{
    public class StatusController : ApiController
    {
        private readonly IDispatcher _dispatcher;
        private readonly ISampler _sampler;
        private readonly ITraceContextAccessor _traceContextAccessor;

        public StatusController(
            IDispatcher dispatcher, 
            ISampler sampler, 
            ITraceContextAccessor traceContextAccessor)
        {
            _dispatcher = dispatcher;
            _sampler = sampler;
            _traceContextAccessor = traceContextAccessor;
        }

        [Route("api/owin/status")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {
            var tracingHandler = new TracingHandler(
                new HttpClientHandler(), 
                _traceContextAccessor,
                _dispatcher,
                _sampler,
                "Ping.API-OWIN");

            var httpClient = new HttpClient(tracingHandler);
            var result = await httpClient.GetAsync(new Uri("http://localhost:5005/api/ping"));
            return Ok(await result.Content.ReadAsStringAsync());
       }
    }
}
