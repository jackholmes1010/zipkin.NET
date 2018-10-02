using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zipkin.NET.Clients.WCF;
using Zipkin.NET.Demo.Connected_Services.DataService;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDispatcher _dispatcher;
        private readonly ISampler _sampler;
        private readonly ITraceContextAccessor _traceContextAccessor;

        public ValuesController(
            IHttpClientFactory httpClientFactory,
            IDispatcher dispatcher,
            ISampler sampler, 
            ITraceContextAccessor traceContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _dispatcher = dispatcher;
            _sampler = sampler;
            _traceContextAccessor = traceContextAccessor;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var httpClient = _httpClientFactory.CreateClient("tracingClient");
            var httpClient2 = _httpClientFactory.CreateClient("tracingClient2");
            var owinClient = _httpClientFactory.CreateClient("owinClient");
            var wcfClient = GetWcfDemoClient();

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost:5005/api/ping"));
            var httpRequest2 = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost:5005/api/ping"));
            var owinHttpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost:9055/api/owin/status"));

            var resultTask = httpClient.SendAsync(httpRequest);
            var result2Task = httpClient2.SendAsync(httpRequest2);
            var owinTask = owinClient.SendAsync(owinHttpRequest);
            var wcfTask = wcfClient.GetDataAsync(1);

            var result = await resultTask;
            var result2 = await result2Task;
            var owinResult = await owinTask;

            return new[]
            {
                "wcfResult", await wcfTask,
                "result", await result.Content.ReadAsStringAsync(),
                "result2", await result2.Content.ReadAsStringAsync(),
                "owinResult", await owinResult.Content.ReadAsStringAsync()
            };
        }

        private DataServiceClient GetWcfDemoClient()
        {
            var wcfClient = new DataServiceClient();
            wcfClient.Endpoint.Address = new EndpointAddress("http://localhost:54069/DataService.svc");
            var endpoint = new TracingEndpointBehavior("wcf-demo", _traceContextAccessor, _sampler, _dispatcher);
            wcfClient.Endpoint.EndpointBehaviors.Add(endpoint);
            return wcfClient;
        }
    }
}
