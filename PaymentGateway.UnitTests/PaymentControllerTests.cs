using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.API.Controllers;
using PaymentGateway.API.Mappers;
using PaymentGateway.API.ViewModels;
using PaymentGateway.Services;
using PaymentGateway.Services.DTOs;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.UnitTests
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentService;
        private IMapper _mapper;
        private PaymentController _paymentController;

        class ControllerTestMapper : Profile
        {
            public ControllerTestMapper()
            {
                CreateMap<PaymentDto, PaymentRequestVm>();
            }
        }

        [SetUp]
        public void Setup()
        {
            _paymentService = new Mock<IPaymentService>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfiles(new Profile[] { new PaymentGatewayAPIMapper(), new ControllerTestMapper() });

            });
            _mapper = mappingConfig.CreateMapper();
            _paymentController = new PaymentController(new Mock<ILogger<PaymentController>>().Object, _paymentService.Object, _mapper);
        }

        [Test]
        public void GetById_SuccessResponse_ForValidId()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var _testPaymentDto = GetTestPaymentDtoWithId(id);
            _paymentService.Setup(ps => ps.GetPaymentDetailsById(It.Is<string>(x => x.Equals(id, StringComparison.InvariantCultureIgnoreCase))))
                .Returns(_testPaymentDto)
                .Verifiable();

            // Act
            var response = _paymentController.Get(id);

            // Assert
            Assert.IsNotNull(response,"Expected response from the API");
            var result = response as OkObjectResult;
            Assert.IsNotNull(result, "OK Response expected from API");
            Assert.AreEqual(200, result.StatusCode);
            var paymentSummary = result.Value as PaymentSummaryVm;
            Assert.IsNotNull(paymentSummary,"Payment Summary expected");
            Assert.AreEqual(_testPaymentDto.Id, paymentSummary.Id, "Id should match");
            Assert.AreEqual($"XXXX-XXXX-XXXX-{_testPaymentDto.CardNumber.Substring(15, 4)}", paymentSummary.MaskedCardNumber, "Masked CardNumber should match");
            _paymentService.Verify();
        }

        [Test]
        public void GetById_NotFoundResponse_ForInValidId()
        {
            // Arrange
            var id = "Invalid_Id";
            _paymentService.Setup(ps => ps.GetPaymentDetailsById(It.Is<string>(x => x.Equals(id, StringComparison.InvariantCultureIgnoreCase))))
                .Returns(default(PaymentDto));

            // Act
            var response = _paymentController.Get(id);

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
            var paymentDTO = this.GetTestPaymentDtoWithId(id);
            var paymentRequestVm = _mapper.Map<PaymentRequestVm>(paymentDTO);

            _paymentService.Setup(ps => ps.SubmitPayment(It.IsAny<PaymentDto>()))
                .ReturnsAsync(paymentDTO).Verifiable();

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
            _paymentService.Verify();
        }


        [Test]
        public async Task PostPayment_BadRequestResponse_ForInValidPaymentRequest()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var paymentDTO = this.GetTestPaymentDtoWithId(id);
            var paymentRequestVm = _mapper.Map<PaymentRequestVm>(paymentDTO);
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

            _paymentService.Verify(ps => ps.SubmitPayment(It.IsAny<PaymentDto>()), Times.Never);
        }

        private PaymentDto GetTestPaymentDtoWithId(string id)
        {
            return new PaymentDto
            {
                Id = id,
                CardNumber = "1234-5678-9012-3456",
                CardHolderName = "John Smith",
                CardType = CardTypeDto.Master,
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