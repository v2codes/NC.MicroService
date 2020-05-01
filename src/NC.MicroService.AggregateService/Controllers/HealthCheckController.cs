using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NC.MicroService.AggregateService.Controllers
{
    [Route("HealthCheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetHealthCheck()
        {
            return Ok("连接正常");
        }
    }
}