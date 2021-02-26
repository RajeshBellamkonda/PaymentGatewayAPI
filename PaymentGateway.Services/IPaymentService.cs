using PaymentGateway.Services.DTOs;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public interface IPaymentService
    {
        PaymentDto GetPaymentDetailsById(string id);
        Task<PaymentDto> SubmitPayment(PaymentDto payment);
    }
}