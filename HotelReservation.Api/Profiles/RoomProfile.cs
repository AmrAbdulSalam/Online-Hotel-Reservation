using AutoMapper;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomDTO , Room>()
                .ForMember(x => x.CreationDate, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore());
        }
    }
}
