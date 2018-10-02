using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;

namespace Zipkin.NET.OWIN.Demo
{
    public class StatusController : ApiController
    {
        [Route("api/owin/status")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {
            // Register zipkin reporter
            var sender = new ZipkinHttpSender("http://localhost:9411");
            var zipkinReporter = new ZipkinReporter(sender); var logger = new ConsoleInstrumentationLogger();
            var reporters = new List<IReporter> { zipkinReporter, new ConsoleReporter() };
            var dispatcher = new AsyncActionBlockDispatcher(reporters, logger);

            var tracingHandler = new TracingHandler(
                new HttpClientHandler(), 
                new CallContextTraceContextAccessor(),
                dispatcher,
                new RateSampler(1f), 
                "reqres-api");

            var httpClient = new HttpClient(tracingHandler);
            var result = await httpClient.GetAsync(new Uri("http://localhost:5005/api/ping"));
            return Ok(await result.Content.ReadAsStringAsync());
       }
    }
}
