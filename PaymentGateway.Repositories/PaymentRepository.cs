using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public async Task<Payment> GetById(string Id)
        {
            //throw new NotImplementedException();
            return null; // To allow testing
        }

        public async Task<Payment> Save(Payment payment)
        {
            //throw new NotImplementedException();
            return null; // To allow testing
        }
    }
}
