using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.ExternalAccess;
using PaymentGateway.Models;
using PaymentGateway.Repositories;
using PaymentGateway.Services;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.UnitTests
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private PaymentService _paymentService;
        private Mock<IPaymentRepository> _paymentRepositoryMock;
        private Mock<IBankClient> _bankClientMock;

        [SetUp]
        public void Setup()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _bankClientMock = new Mock<IBankClient>();
            _paymentService = new PaymentService(new Mock<ILogger<PaymentService>>().Object, _paymentRepositoryMock.Object, _bankClientMock.Object);
        }

        [Test]
        public async Task GetPaymentDetailsById_ReturnsPaymentDetailsFromDb()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var expectedPaymentDetail = GetTestPaymentWithId(id);
            _paymentRepositoryMock.Setup(pr => pr.GetById(It.Is<string>(i => i.Equals(id, StringComparison.InvariantCultureIgnoreCase))))
                .ReturnsAsync(expectedPaymentDetail);

            // Act
            var paymentDetails = await _paymentService.GetPaymentDetailsById(id);

            // Assert
            Assert.IsNotNull(paymentDetails);
            Assert.AreEqual(expectedPaymentDetail, paymentDetails, "Payment details are not as expected");
            _bankClientMock.Verify(bc => bc.GetPaymentDetails(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetPaymentDetailsById_ReturnsPaymentDetailsFromBank_WhenNotFoundFromDb()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var expectedPaymentDetail = GetTestPaymentWithId(id);
            _paymentRepositoryMock.Setup(pr => pr.GetById(It.Is<string>(i => i.Equals(id, StringComparison.InvariantCultureIgnoreCase))))
                .ReturnsAsync(default(Payment));

            _bankClientMock.Setup(bc => bc.GetPaymentDetails(It.Is<string>(i => i.Equals(id, StringComparison.InvariantCultureIgnoreCase))))
                .ReturnsAsync(expectedPaymentDetail);

            // Act
            var paymentDetails = await _paymentService.GetPaymentDetailsById(id);

            // Assert
            Assert.IsNotNull(paymentDetails);
            Assert.AreEqual(expectedPaymentDetail, paymentDetails, "Payment details are not as expected");
            _paymentRepositoryMock.Verify(pr => pr.GetById(It.IsAny<string>()), Times.Once);
            _bankClientMock.Verify(bc => bc.GetPaymentDetails(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ProcessPayment_ReturnsPaymentDetailsWithStatus()
        {
            // Arrange
            var paymentToProcess = GetTestPaymentWithId(null);
            paymentToProcess.Status = null;
            paymentToProcess.Id = null;

            var expectedId = Guid.NewGuid().ToString();
            var expectedPaymentDetail = GetTestPaymentWithId(expectedId);
            _bankClientMock.Setup(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))))
                .ReturnsAsync(expectedPaymentDetail)
                .Verifiable();

            // Act
            var paymentDetails = await _paymentService.ProcessPayment(paymentToProcess);

            // Assert
            Assert.IsNotNull(paymentDetails);
            Assert.AreEqual(expectedPaymentDetail, paymentDetails, "Payment details are not as expected");
            _bankClientMock.Verify(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))), Times.Once);

        }

        [Test]
        public async Task ProcessPayment_ReturnsPaymentDetailsWithStatus_AndSavesToDB()
        {
            // Arrange
            var paymentToProcess = GetTestPaymentWithId(null);
            paymentToProcess.Status = null;
            paymentToProcess.Id = null;

            var expectedId = Guid.NewGuid().ToString();
            var expectedPaymentDetail = GetTestPaymentWithId(expectedId);
            _bankClientMock.Setup(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))))
                .ReturnsAsync(expectedPaymentDetail)
                .Verifiable();

            _paymentRepositoryMock.Setup(pr => pr.Save(It.Is<Payment>(p => p.Equals(expectedPaymentDetail))))
                .ReturnsAsync(expectedPaymentDetail)
                .Verifiable();


            // Act
            var paymentDetails = await _paymentService.ProcessPayment(paymentToProcess);

            // Assert
            Assert.IsNotNull(paymentDetails);
            Assert.AreEqual(expectedPaymentDetail, paymentDetails, "Payment details are not as expected");
            _bankClientMock.Verify(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))), Times.Once);
            _paymentRepositoryMock.Verify(pr => pr.Save(It.Is<Payment>(p => p.Equals(expectedPaymentDetail))), Times.Once);
        }

        [Test]
        public async Task ProcessPayment_WhenBankException_IsHandled()
        {
            // Arrange
            var paymentToProcess = GetTestPaymentWithId(null);
            paymentToProcess.Status = null;

            var expectedPaymentDetail = GetTestPaymentWithId(null);
            expectedPaymentDetail.Status = "Error processing payment.";


            _bankClientMock.Setup(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))))
                .ThrowsAsync(new Exception("Error Processing Payment"))
                .Verifiable();

            _paymentRepositoryMock.Setup(pr => pr.Save(It.Is<Payment>(p => p.Equals(expectedPaymentDetail))))
                .ThrowsAsync(new Exception("Error Saving Payment"))
                .Verifiable();

            // Act
            var paymentDetails = await _paymentService.ProcessPayment(paymentToProcess);

            // Assert
            Assert.IsNotNull(paymentDetails);
            Assert.AreEqual(expectedPaymentDetail.Id, paymentDetails.Id, "The id should be null");
            Assert.AreEqual(expectedPaymentDetail.Status, paymentDetails.Status, "The status should be equal");
            _bankClientMock.Verify(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))), Times.Once);
            _paymentRepositoryMock.Verify(pr => pr.Save(It.Is<Payment>(p => p.Equals(expectedPaymentDetail))), Times.Never);
        }

        [Test]
        public async Task ProcessPayment_WhenDBException_IsHandled()
        {
            // Arrange
            var paymentToProcess = GetTestPaymentWithId(null);
            paymentToProcess.Status = null;

            var expectedPaymentDetail = GetTestPaymentWithId(null);
            expectedPaymentDetail.Status = "Error processing payment.";

            _bankClientMock.Setup(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))))
                .ReturnsAsync(expectedPaymentDetail)
                .Verifiable();

            _paymentRepositoryMock.Setup(pr => pr.Save(It.Is<Payment>(p => p.Equals(expectedPaymentDetail))))
                .ThrowsAsync(new Exception("Error Saving Payment"))
                .Verifiable();

            // Act
            var paymentDetails = await _paymentService.ProcessPayment(paymentToProcess);

            // Assert
            Assert.IsNotNull(paymentDetails);
            Assert.AreEqual(expectedPaymentDetail.Id, paymentDetails.Id, "The id should not be null");
            Assert.AreEqual(expectedPaymentDetail.Status, paymentDetails.Status, "The status should be equal");
            _bankClientMock.Verify(bc => bc.ProcessPayment(It.Is<Payment>(p => p.Equals(paymentToProcess))), Times.Once);
            _paymentRepositoryMock.Verify(pr => pr.Save(It.Is<Payment>(p => p.Equals(expectedPaymentDetail))), Times.Once);
        }

        private Payment GetTestPaymentWithId(string id)
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
