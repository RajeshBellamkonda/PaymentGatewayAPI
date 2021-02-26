using PaymentGateway.Services.DTOs;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        public PaymentDto GetPaymentDetailsById(string id)
        {
            return new PaymentDto();
        }

        public async Task<PaymentDto> SubmitPayment(PaymentDto payment)
        {
            payment.Id = Guid.NewGuid().ToString();
            payment.Status = "Success";
            return payment;
        }
    }
}
