using AutoMapper;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Profiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<ReservationDTO , Reservation>()
                .ForMember(x => x.ReferenceceNo, opt => opt.Ignore())
                .ForMember(x => x.Price, opt => opt.Ignore());
        }
    }
}
