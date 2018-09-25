using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zipkin.NET.Clients.WCF;
using Zipkin.NET.Demo.Connected_Services.DataService;
using Zipkin.NET.Middleware.TraceAccessors;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValuesController(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var httpClient = _httpClientFactory.CreateClient("tracingClient");
            var httpClient2 = _httpClientFactory.CreateClient("tracingClient2");
            var owinClient = _httpClientFactory.CreateClient("owinClient");
            var wcfClient = GetWcfDemoClient();

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://reqres.in/api/users?delay=3"));
            var httpRequest2 = new HttpRequestMessage(HttpMethod.Get, new Uri("https://reqres.in/api/users?delay=2"));
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
            var traceContextAccessor = new HttpContextTraceContextAccessor(_httpContextAccessor);
            wcfClient.Endpoint.Address = new EndpointAddress("http://localhost:54069/DataService.svc");
            var endpoint = new TracingEndpointBehavior("wcf-demo", traceContextAccessor);
            wcfClient.Endpoint.EndpointBehaviors.Add(endpoint);
            return wcfClient;
        }
    }
}
