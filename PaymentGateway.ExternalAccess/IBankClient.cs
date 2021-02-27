using PaymentGateway.Models;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.ExternalAccess
{
    public interface IBankClient
    {
        Task<Payment> ProcessPayment(Payment payment);
        Task<Payment> GetPaymentDetails(string id);
    }
}
