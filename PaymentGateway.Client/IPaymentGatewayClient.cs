using PaymentGateway.Client.Models;
using System.Threading.Tasks;

namespace PaymentGateway.Client
{
    public interface IPaymentGatewayClient
    {
        Task<PaymentSummary> ProcessPayment(PaymentRequest payment);
        Task<PaymentSummary> GetPaymentDetails(string id);
    }
}
