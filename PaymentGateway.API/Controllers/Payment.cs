using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentGateway.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {

        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        // GET api/<Payment>/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            _logger.LogDebug($"Get requested with id {id}");
            return "value";
        }

        // POST api/<Payment>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
