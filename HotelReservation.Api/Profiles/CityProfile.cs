using AutoMapper;
using HotelReservation.Api.Models.CityModel;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<CityDTO , City>();
        }
    }
}
