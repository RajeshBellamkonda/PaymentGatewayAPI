using AutoMapper;
using PaymentGateway.API.ViewModels;
using PaymentGateway.Services.DTOs;

namespace PaymentGateway.API.Mappers
{
    public class PaymentGatewayAPIMapper: Profile
    {
        public PaymentGatewayAPIMapper()
        {
            CreateMap<PaymentVm, PaymentDto>();
            CreateMap<PaymentDto, PaymentVm>();
            CreateMap<CardTypeVm, CardTypeDto>();
            CreateMap<CardTypeDto, CardTypeVm>();
        }

    }
}
