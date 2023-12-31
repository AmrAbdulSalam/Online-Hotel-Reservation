using AutoMapper;
using HotelReservation.Db.Models;

namespace HotelReservation.Db.Profiles
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            CreateMap<Domain.Models.Hotel, Hotel>();

            CreateMap<Hotel, Domain.Models.Hotel>();
        }
    }
}
