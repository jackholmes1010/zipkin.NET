using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zipkin.NET.Clients.WCF;
using Zipkin.NET.Demo.Connected_Services.DataService;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;

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
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/todos/1"));
            var httpRequest2 = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/todos/2"));

	        var wcfClient = new DataServiceClient();
			wcfClient.Endpoint.Address = new EndpointAddress("http://localhost:54069/DataService.svc");
	        wcfClient.Endpoint.EndpointBehaviors.Add(new ZipkinEndpointBehavior("data-service",
		        new Reporter(new HttpSender("http://localhost:8888")),
		        new HttpContextTraceContextAccessor(new HttpContextAccessor())));

	        var wcfResult = wcfClient.GetDataAsync(1); 

	        var resultTask = httpClient.SendAsync(httpRequest);
	        var result2Task = httpClient2.SendAsync(httpRequest2);

			var result = await resultTask;
	        var result2 = await result2Task;

			return new string[]
            {
	            "wcfResult", await wcfResult,
				"result", await result.Content.ReadAsStringAsync(),
                "result2", await result2.Content.ReadAsStringAsync(),
            };
        }
    }
}
