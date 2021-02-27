using AutoMapper;
using PaymentGateway.API.ViewModels;
using PaymentGateway.Models;

namespace PaymentGateway.API.Mappers
{
    public class PaymentGatewayAPIMapper: Profile
    {
        public PaymentGatewayAPIMapper()
        {
            CreateMap<PaymentRequestVm, Payment>();
            CreateMap<Payment, PaymentSummaryVm>();
        }

    }
}
