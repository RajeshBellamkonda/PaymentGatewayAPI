using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Repositories
{
    /// <summary>
    /// Payment Repository to store payments
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        /// Retrieves a payment for a unique Id.
        /// </summary>
        /// <param name="Id">Unique Id for the payment.</param>
        /// <returns>Payment</returns>
        Task<Payment> GetById(string Id);

        /// <summary>
        /// Saves a payment.
        /// </summary>
        /// <param name="payment">payment with details</param>
        /// <returns>Payment</returns>
        Task<Payment> Save(Payment payment);
    }
}
