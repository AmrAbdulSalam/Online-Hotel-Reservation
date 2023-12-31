using AutoMapper;
using HotelReservation.Db.Models;

namespace HotelReservation.Db.Profiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Domain.Models.Reservation, Reservation>();

            CreateMap<Reservation, Domain.Models.Reservation>();
        }
    }
}
