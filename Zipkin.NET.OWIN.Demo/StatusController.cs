using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Zipkin.NET.OWIN.Demo
{
    public class StatusController : ApiController
    {
        [Route("api/owin/status")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(new Uri("https://reqres.in/api/users?page=2"));
            return Ok(await result.Content.ReadAsStringAsync());
        }
    }
}
