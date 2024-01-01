using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface IFeaturedDealService
    {
        Task<List<FeaturedDeal>> GetAllFeaturedDealsAsync();

        Task<FeaturedDeal> GetFeaturedDealByIdAsync(int featuredDealId);

        Task AddFeaturedDealAsync(FeaturedDeal featuredDeal);

        Task UpdateFeaturedDealAsync(FeaturedDeal featuredDeal);

        Task DeleteFeaturedDealAsync(int featuredDealId);

        Task<bool> FeaturedDealExists(int featuredDealId);
    }
}
