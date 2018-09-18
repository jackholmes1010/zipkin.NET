using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zipkin.NET.Demo.Connected_Services.DataService;

namespace Zipkin.NET.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ValuesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var httpClient = _httpClientFactory.CreateClient("tracingClient");
            var httpClient2 = _httpClientFactory.CreateClient("tracingClient2");
            var owinClient = _httpClientFactory.CreateClient("owinClient");
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/todos/1"));
            var httpRequest2 = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/todos/2"));
            var owinHttpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost:9055/api/owin/status"));

            var wcfClient = new DataServiceClient();
            wcfClient.Endpoint.Address = new EndpointAddress("http://localhost:54069/DataService.svc");
            //var endpoint = new ZipkinEndpointBehavior("data-service",
            //    new Reporter(new HttpSender("http://localhost:9411")),
            //    new HttpContextTraceContextAccessor(new HttpContextAccessor()));

            //wcfClient.Endpoint.EndpointBehaviors.Add(endpoint);

            var wcfResult = wcfClient.GetDataAsync(1); 

            var resultTask = httpClient.SendAsync(httpRequest);
            var result2Task = httpClient2.SendAsync(httpRequest2);
            var owinTask = owinClient.SendAsync(owinHttpRequest);

            var result = await resultTask;
            var result2 = await result2Task;
            var owinResult = await owinTask;

            return new string[]
            {
                "wcfResult", await wcfResult,
                "result", await result.Content.ReadAsStringAsync(),
                "result2", await result2.Content.ReadAsStringAsync(),
                "owinResult", await owinResult.Content.ReadAsStringAsync()
            };
        }
    }
}
