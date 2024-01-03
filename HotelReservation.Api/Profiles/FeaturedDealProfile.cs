using AutoMapper;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Profiles
{
    public class FeaturedDealProfile : Profile
    {
        public FeaturedDealProfile()
        {
            CreateMap<FeaturedDealDTO, FeaturedDeal>();
        }
    }
}
