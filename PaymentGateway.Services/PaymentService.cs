using PaymentGateway.ExternalAccess;
using PaymentGateway.Models;
using PaymentGateway.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        private const string ErrorMessage = "Error processing payment.";
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBankClient _bankClient;

        public PaymentService(ILogger<PaymentService> logger, IPaymentRepository paymentRepository, IBankClient bankClient)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
            _bankClient = bankClient;
        }

        public async Task<Payment> GetPaymentDetailsById(string id)
        {
            _logger.LogDebug($"Getting payment details by Id {id}");
            var paymentdetails = await _paymentRepository.GetById(id);
            if (paymentdetails == null)
            {
                _logger.LogInformation($"Payment details for Id {id} are not found locally, retrieving from bank.");
                paymentdetails = await _bankClient.GetPaymentDetails(id);
            }
            // Decrypt the payment details if encrypted before saving
            return paymentdetails;
        }

        public async Task<Payment> ProcessPayment(Payment payment)
        {
            try
            {
                //process payment from bank
                var processedPayment = await _bankClient.ProcessPayment(payment);
                _logger.LogInformation($"Processed payment for {payment.MaskedCardNumber},  Status: {processedPayment.Status}, id: {processedPayment.Id}");

                try
                {
                    // TODO: Encrypt the card details before saving.
                    await _paymentRepository.Save(processedPayment);
                    _logger.LogDebug($"Saved payment details for {payment.MaskedCardNumber},  Status: {processedPayment.Status}, id: {processedPayment.Id}");
                }
                catch (Exception repoEx) // TODO : This can be simplified by throwing & catching custom exceptions
                {
                    _logger.LogError(new EventId(2), repoEx, "Error saving processed payment");
                }
                return processedPayment;
            }
            catch (Exception ex) // TODO : This can be simplified by throwing & catching custom exceptions
            {
                _logger.LogError(new EventId(1), ex, ErrorMessage);
                payment.Status = ErrorMessage;
            }
            return payment;
        }
    }
}
