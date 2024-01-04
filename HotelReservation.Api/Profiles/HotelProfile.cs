using AutoMapper;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Profiles
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            CreateMap<HotelDTO, Hotel>()
                .ForMember(x => x.CreationDate, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore());
        }
    }
}
