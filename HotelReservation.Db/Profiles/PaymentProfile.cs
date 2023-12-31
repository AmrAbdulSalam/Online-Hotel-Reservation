using AutoMapper;
using HotelReservation.Db.Models;

namespace HotelReservation.Db.Profiles
{
    public class PaymentProfile :Profile
    {
        public PaymentProfile()
        {
            CreateMap<Domain.Models.Payment, Payment>();

            CreateMap<Payment, Domain.Models.Payment>();
        }
    }
}
