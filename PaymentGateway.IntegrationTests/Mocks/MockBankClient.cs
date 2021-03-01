using PaymentGateway.ExternalAccess;
using PaymentGateway.Models;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.IntegrationTests.Mocks
{
    public class MockBankClient : IBankClient
    {
        public const string INVALID_ID = "InvalidId";
        public MockBankClient()
        {
        }

        public async Task<Payment> GetPaymentDetails(string id)
        {
            if (id.Equals(INVALID_ID, StringComparison.InvariantCultureIgnoreCase))
            {
                return default;
            }
            return GetMockPayment(id);
        }

        public async Task<Payment> ProcessPayment(Payment payment)
        {
            return GetMockPayment(Guid.NewGuid().ToString());
        }

        private Payment GetMockPayment(string id)
        {
            return new Payment
            {
                Id = id,
                CardNumber = "1234-5678-9012-3456",
                CardHolderName = "John Smith",
                CardType = CardType.Master,
                Cvv = 123,
                Status = "Success",
                Amount = 123,
                ExpiryMonth = 9,
                ExpiryYear = 2021,
                Currency = "GBP",
                Address1 = "Line 1",
                City = "London",
                Country = "UK",
                PostCode = "EX17 1KB"
            };
        }
    }
}