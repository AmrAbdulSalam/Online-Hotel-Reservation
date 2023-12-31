using AutoMapper;
using HotelReservation.Db.Models;

namespace HotelReservation.Db.Profiles
{
    public class FeaturedDealProfile : Profile
    {
        public FeaturedDealProfile()
        {
            CreateMap<Domain.Models.FeaturedDeal, FeaturedDeal>();

            CreateMap<FeaturedDeal, Domain.Models.FeaturedDeal>();
        }
    }
}
