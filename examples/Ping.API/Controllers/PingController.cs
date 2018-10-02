using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult Get()
        {
            Thread.Sleep(20);
            return Ok("pong");
        }
    }
}
