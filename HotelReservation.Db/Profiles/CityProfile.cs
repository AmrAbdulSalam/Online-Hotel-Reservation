using AutoMapper;
using HotelReservation.Db.Models;

namespace HotelReservation.Db.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Domain.Models.City, City>();

            CreateMap<City, Domain.Models.City>();
        }
    }
}
