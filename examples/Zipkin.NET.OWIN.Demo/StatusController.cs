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
        private readonly ISpanContextAccessor _spanContextAccessor;

        public StatusController(
            IDispatcher dispatcher, 
            ISampler sampler, 
            ISpanContextAccessor spanContextAccessor)
        {
            _dispatcher = dispatcher;
            _sampler = sampler;
            _spanContextAccessor = spanContextAccessor;
        }

        [Route("api/owin/status")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {
            var tracingHandler = new TracingHandler(
                new HttpClientHandler(), 
                _spanContextAccessor,
                _dispatcher,
                _sampler,
                "Ping.API-OWIN");

            var httpClient = new HttpClient(tracingHandler);
            var result = await httpClient.GetAsync(new Uri("http://localhost:5005/api/ping"));
            return Ok(await result.Content.ReadAsStringAsync());
       }
    }
}
