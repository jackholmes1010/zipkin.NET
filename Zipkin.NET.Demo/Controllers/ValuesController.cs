using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zipkin.NET.Instrumentation;

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
	        var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("http://www.google.com"));
	        var httpRequest2 = new HttpRequestMessage(HttpMethod.Get, new Uri("http://www.google.com"));

			var resultTask = httpClient.SendAsync(httpRequest);
			var result2Task = httpClient2.SendAsync(httpRequest2);

            var result = await resultTask;
            var result2 = await result2Task;

            return new string[]
            {
                "result", result.Content.ToString(),
                "result2", result2.Content.ToString(),
            };
        }
    }
}
