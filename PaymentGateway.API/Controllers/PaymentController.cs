﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.API.ViewModels;
using PaymentGateway.Models;
using PaymentGateway.Services;
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
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService, IMapper mapper)
        {
            _logger = logger;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        // GET api/<Payment>/5
        /// <summary>
        /// Get Payment details for an id.
        /// </summary>
        /// <param name="id">unique id for the payment details.</param>
        /// <returns>Payment Summary View Model.</returns>
        /// <response code="401">Not Authorised.</response>
        /// <response code="200">Returns payment summary for the requested id.</response>
        /// <response code="404">If the payment details are not found.</response>    
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            _logger.LogDebug($"Get requested with id {id}");
            var payment = await _paymentService.GetPaymentDetailsById(id);
            if (payment == null)
            {
                var message = $"Requested payment details with Id {id} is not found";
                _logger.LogDebug(message);
                return NotFound(message);
            }
            var paymentSummaryVm = _mapper.Map<PaymentSummaryVm>(payment);
            _logger.LogDebug($"Successfully retrieved payment details with id {id}");
            return Ok(paymentSummaryVm);
        }

        // POST api/<Payment>
        /// <summary>
        /// Processes the payment with the given details.
        /// </summary>
        /// <param name="paymentRequest">The payment details to be processed.</param>
        /// <returns>Payment Summary View Model.</returns>
        /// <response code="401">Not Authorised.</response>
        /// <response code="200">Returns payment summary after processing the request.</response>
        /// <response code="400">If the payment request is invalid.</response>    
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentRequestVm paymentRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Bad Request: {JsonConvert.SerializeObject(ModelState)}");
                return BadRequest(ModelState);
            }
            var payment = _mapper.Map<Payment>(paymentRequest);
            _logger.LogDebug($"Submitting Payment request for : {payment.MaskedCardNumber}");
            var paymentDto = await _paymentService.ProcessPayment(payment);
            var paymentSummaryVm = _mapper.Map<PaymentSummaryVm>(paymentDto);
            _logger.LogDebug($"Processed Payment for : {paymentSummaryVm.MaskedCardNumber}");
            return base.Ok(paymentSummaryVm);
        }
    }
}
