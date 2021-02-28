using Bank.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
       
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Ok(new Payment
            {
                Id = id,
                CardNumber = "1234-5678-9012-3456",
                CardHolderName = "John Smith",
                CardType = CardType.Master,
                Cvv = 123,
                Status = "Success",
                Amount = 123,
                ExpiryMonth = 9,
                ExpiryYear = 2021,
                Currency = "GBP",
                Address1 = "Line 1",
                City = "London",
                Country = "UK",
                PostCode = "EX17 1KB"
            });
        }

        [HttpPost]
        public IActionResult Post([FromBody] Payment paymentRequest)
        {
            paymentRequest.Status = "Success";
            paymentRequest.Id = Guid.NewGuid().ToString();
            return Ok(paymentRequest);
        }
    }
}
