using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.API.ViewModels;
using PaymentGateway.Services;
using PaymentGateway.Services.DTOs;
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
        public IActionResult Get(string id)
        {
            _logger.LogDebug($"Get requested with id {id}");
            var paymentDetails = _paymentService.GetPaymentDetailsById(id);
            if (paymentDetails == null)
            {
                var message = $"Requested payment details with Id {id} is not found";
                _logger.LogDebug(message);
                return NotFound(message);
            }
            var paymentVm = _mapper.Map<PaymentSummaryVm>(paymentDetails);
            _logger.LogDebug($"Requested payment details with id {id} : {JsonConvert.SerializeObject(paymentVm)}");
            return Ok(paymentVm);
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
            _logger.LogDebug($"Requested payment with details: {JsonConvert.SerializeObject(paymentRequest)}");
            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Bad Request: {JsonConvert.SerializeObject(ModelState)}");
                return BadRequest(ModelState);
            }
            var paymentDto = await _paymentService.SubmitPayment(_mapper.Map<PaymentDto>(paymentRequest));
            var paymentSummaryVm = _mapper.Map<PaymentSummaryVm>(paymentDto);
            _logger.LogDebug($"Processed Payment: {JsonConvert.SerializeObject(paymentSummaryVm)}");
            return base.Ok(paymentSummaryVm);
        }
    }
}
