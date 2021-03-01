using Newtonsoft.Json;
using NUnit.Framework;
using PaymentGateway.API.ViewModels;
using PaymentGateway.IntegrationTests.Mocks;
using PaymentGateway.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
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

        [Test]
        public async Task Post_Payment_With_ValidDetails_Returns_Success()
        {
            // Arrange
            var paymentRequest = GetPaymentRequest();
            // Act
            var paymentResponse = await Client.PostAsync("payment", new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json"));

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, paymentResponse.StatusCode);
            Assert.IsNotNull(paymentResponse);

            var paymentSummary = JsonConvert.DeserializeObject<PaymentSummaryVm>(await paymentResponse.Content.ReadAsStringAsync());
            Assert.IsNotNull(paymentSummary);
            Assert.IsNotNull(paymentSummary.Id);
            Assert.IsNotNull(paymentSummary.Status);
        }

        [Test]
        public async Task Post_Payment_With_InValidDetails_Returns_BadRequest()
        {
            // Arrange
            var paymentRequest = GetPaymentRequest();
            paymentRequest.CardNumber = null;

            // Act
            var paymentResponse = await Client.PostAsync("payment", new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json"));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, paymentResponse.StatusCode);
            Assert.IsNotNull(paymentResponse);

            var errorMessage = await paymentResponse.Content.ReadAsStringAsync();
            Assert.IsNotNull(errorMessage);
            Assert.IsTrue(errorMessage.Contains("The CardNumber field is required.", StringComparison.InvariantCultureIgnoreCase));

        }

        private PaymentRequestVm GetPaymentRequest()
        {
            return new PaymentRequestVm
            {
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
