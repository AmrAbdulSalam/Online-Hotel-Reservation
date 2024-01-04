using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Application.Services
{
    public class FeaturedDealService : IFeaturedDealService
    {
        private readonly IFeaturedDealRepository _featuredDealRepository;

        public FeaturedDealService(IFeaturedDealRepository featuredDealRepository)
        {
            _featuredDealRepository = featuredDealRepository;
        }

        public async Task<int> AddFeaturedDealAsync(FeaturedDeal featuredDeal)
        {
            return await _featuredDealRepository.AddFeaturedDealAsync(featuredDeal);
        }

        public async Task DeleteFeaturedDealAsync(int featuredDealId)
        {
            await _featuredDealRepository.DeleteFeaturedDealAsync(featuredDealId);
        }

        public async Task<bool> FeaturedDealExists(int featuredDealId)
        {
            return await _featuredDealRepository.FeaturedDealExists(featuredDealId);
        }

        public async Task<List<FeaturedDeal>> GetAllFeaturedDealsAsync(int pageNumber, int pageSize)
        {
            return await _featuredDealRepository.GetAllFeaturedDealsAsync(pageNumber, pageSize);
        }

        public async Task<FeaturedDeal> GetFeaturedDealByIdAsync(int featuredDealId)
        {
            return await _featuredDealRepository.GetFeaturedDealByIdAsync(featuredDealId);
        }

        public async Task UpdateFeaturedDealAsync(FeaturedDeal featuredDeal)
        {
            await _featuredDealRepository.UpdateFeaturedDealAsync(featuredDeal);
        }
    }
}
