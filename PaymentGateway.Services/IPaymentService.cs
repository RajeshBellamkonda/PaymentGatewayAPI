using PaymentGateway.Models;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public interface IPaymentService
    {
        Task<Payment> GetPaymentDetailsById(string id);
        Task<Payment> ProcessPayment(Payment payment);
    }
}