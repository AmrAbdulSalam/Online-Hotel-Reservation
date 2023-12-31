using AutoMapper;
using HotelReservation.Db.Models;

namespace HotelReservation.Db.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Domain.Models.Room, Room>();

            CreateMap<Room, Domain.Models.Room>();
        }
    }
}
