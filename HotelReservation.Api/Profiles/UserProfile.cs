using AutoMapper;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO , User>();
        }
    }
}
