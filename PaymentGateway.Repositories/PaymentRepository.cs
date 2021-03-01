using PaymentGateway.Models;
using System.Threading.Tasks;

namespace PaymentGateway.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public async Task<Payment> GetById(string Id)
        {
            //throw new NotImplementedException();
            return await Task.Run(() => default(Payment)); // To allow testing
        }

        public async Task<Payment> Save(Payment payment)
        {
            //throw new NotImplementedException();
            return await Task.Run(() => default(Payment)); // To allow testing
        }
    }
}
