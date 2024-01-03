using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.RepositoryInterfaces
{
    public interface IFeaturedDealRepository
    {
        Task<List<FeaturedDeal>> GetAllFeaturedDealsAsync(int pageNumber, int pageSize);

        Task<FeaturedDeal> GetFeaturedDealByIdAsync(int featuredDealId);

        Task<int> AddFeaturedDealAsync(FeaturedDeal featuredDeal);

        Task UpdateFeaturedDealAsync(FeaturedDeal featuredDeal);

        Task DeleteFeaturedDealAsync(int featuredDealId);

        Task<bool> FeaturedDealExists(int featuredDealId);
    }
}
