using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.API.Controllers;
using PaymentGateway.API.Mappers;
using PaymentGateway.API.ViewModels;
using PaymentGateway.Models;
using PaymentGateway.Services;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.UnitTests
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private IMapper _mapper;
        private Mock<IPaymentService> _paymentServiceMock;
        private PaymentController _paymentController;

        class ControllerTestMapper : Profile
        {
            public ControllerTestMapper()
            {
                CreateMap<Payment, PaymentRequestVm>();
            }
        }

        [SetUp]
        public void Setup()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfiles(new Profile[] { new PaymentGatewayAPIMapper(), new ControllerTestMapper() });

            });
            _mapper = mappingConfig.CreateMapper();
            _paymentController = new PaymentController(new Mock<ILogger<PaymentController>>().Object, _paymentServiceMock.Object, _mapper);
        }

        [Test]
        public async Task GetById_SuccessResponse_ForValidId()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var _testPayment = GetTestPaymentWithId(id);
            _paymentServiceMock.Setup(ps => ps.GetPaymentDetailsById(It.Is<string>(x => x.Equals(id, StringComparison.InvariantCultureIgnoreCase))))
                .ReturnsAsync(_testPayment)
                .Verifiable();

            // Act
            var response = await _paymentController.Get(id);

            // Assert
            Assert.IsNotNull(response,"Expected response from the API");
            var result = response as OkObjectResult;
            Assert.IsNotNull(result, "OK Response expected from API");
            Assert.AreEqual(200, result.StatusCode);
            var paymentSummary = result.Value as PaymentSummaryVm;
            Assert.IsNotNull(paymentSummary,"Payment Summary expected");
            Assert.AreEqual(_testPayment.Id, paymentSummary.Id, "Id should match");
            Assert.AreEqual($"XXXX-XXXX-XXXX-{_testPayment.CardNumber.Substring(15, 4)}", paymentSummary.MaskedCardNumber, "Masked CardNumber should match");
            _paymentServiceMock.Verify();
        }

        [Test]
        public async Task GetById_NotFoundResponse_ForInValidId()
        {
            // Arrange
            var id = "Invalid_Id";
            _paymentServiceMock.Setup(ps => ps.GetPaymentDetailsById(It.Is<string>(x => x.Equals(id, StringComparison.InvariantCultureIgnoreCase))))
                .ReturnsAsync(default(Payment));

            // Act
            var response = await _paymentController.Get(id);

            // Assert
            Assert.IsNotNull(response, "Expected response from the API");
            var result = response as NotFoundObjectResult;
            Assert.IsNotNull(result, "NotFound Response expected from API");
            Assert.AreEqual(404, result.StatusCode, "Expected status code 404");
            var paymentSummary = result.Value as PaymentSummaryVm;
            Assert.IsNull(paymentSummary, "PaymentSummary should be null");
        }

        [Test]
        public async Task PostPayment_SuccessResponse_ForValidPaymentRequest()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var Payment = this.GetTestPaymentWithId(id);
            var paymentRequestVm = _mapper.Map<PaymentRequestVm>(Payment);

            _paymentServiceMock.Setup(ps => ps.ProcessPayment(It.IsAny<Payment>()))
                .ReturnsAsync(Payment).Verifiable();

            // Act
            var response = await _paymentController.Post(paymentRequestVm);

            // Assert
            Assert.IsNotNull(response, "Expected response from the API");
            var result = response as OkObjectResult;
            Assert.IsNotNull(result, "OK Response expected from API");
            Assert.AreEqual(200, result.StatusCode);
            var paymentSummary = result.Value as PaymentSummaryVm;
            Assert.IsNotNull(paymentSummary, "Payment Summary expected");
            Assert.IsNotNull(paymentSummary.Id,"Unique Id expected");
            Assert.IsNotNull(paymentSummary.Status,"Payment status expected");
            _paymentServiceMock.Verify();
        }


        [Test]
        public async Task PostPayment_BadRequestResponse_ForInValidPaymentRequest()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var Payment = this.GetTestPaymentWithId(id);
            var paymentRequestVm = _mapper.Map<PaymentRequestVm>(Payment);
            paymentRequestVm.CardNumber = null;
            _paymentController.ModelState.AddModelError("CardNumber", "Invalid Card Number");
            
            // Act
            var response = await _paymentController.Post(paymentRequestVm);

            // Assert
            Assert.IsNotNull(response, "Expected response from the API");
            var result = response as BadRequestObjectResult;
            Assert.IsNotNull(result,"BadRequest expected from API");
            Assert.AreEqual(400, result.StatusCode, "BadRequest status code expected");
            var paymentSummary = result.Value as PaymentSummaryVm;
            Assert.IsNull(paymentSummary,"PaymentSummary should be null");

            _paymentServiceMock.Verify(ps => ps.ProcessPayment(It.IsAny<Payment>()), Times.Never);
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