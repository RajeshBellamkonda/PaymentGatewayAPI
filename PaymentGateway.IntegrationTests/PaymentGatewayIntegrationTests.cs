using Newtonsoft.Json;
using NUnit.Framework;
using PaymentGateway.API.ViewModels;
using PaymentGateway.IntegrationTests.Mocks;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.IntegrationTests
{
    [TestFixture]
    public class PaymentGatewayIntegrationTests : TestServerFixture
    {
        public void Setup()
        {

        }

        [Test]
        public async Task Get_Payment_With_ValidId_Returns_Payment()
        {
            // Arrange
            var paymentId = Guid.NewGuid().ToString();
            // Act
            var paymentResponse = await Client.GetAsync($"/payment/{paymentId}");

            // Assert
            paymentResponse.EnsureSuccessStatusCode();
            Assert.IsNotNull(paymentResponse);

            var paymentSummary = JsonConvert.DeserializeObject<PaymentSummaryVm>(await paymentResponse.Content.ReadAsStringAsync());
            Assert.AreEqual(paymentId, paymentSummary.Id);
        }

        [Test]
        public async Task Get_Payment_With_InvalidValidId_Returns_BadRequest()
        {
            // Arrange
            var paymentId = MockBankClient.INVALID_ID;
            // Act
            var paymentResponse = await Client.GetAsync($"/payment/{paymentId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, paymentResponse.StatusCode);
            Assert.IsNotNull(paymentResponse);

            var errorMessage = await paymentResponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(errorMessage);
            Assert.IsTrue(errorMessage.Equals("Requested payment details with Id InvalidId is not found", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
