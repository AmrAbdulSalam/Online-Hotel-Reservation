using AutoMapper;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Profiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<PaymentDTO , Payment>();
        }
    }
}
