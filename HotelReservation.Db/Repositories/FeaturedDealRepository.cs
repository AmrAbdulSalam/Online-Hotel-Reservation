using AutoMapper;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    public class FeaturedDealRepository : IFeaturedDealRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        public FeaturedDealRepository(HotelReservationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> AddFeaturedDealAsync(FeaturedDeal featuredDeal)
        {
            if (featuredDeal == null)
            {
                throw new ArgumentNullException(nameof(featuredDeal));
            }

            var mappedFeaturedDeal = _mapper.Map<Models.FeaturedDeal>(featuredDeal);

            await _dbContext.FeaturedDeals.AddAsync(mappedFeaturedDeal);

            _dbContext.SaveChanges();

            return mappedFeaturedDeal.Id;
        }

        public async Task DeleteFeaturedDealAsync(int featuredDealId)
        {
            var featuredDeal = await GetFeaturedDealByIdAsync(featuredDealId);

            var mappedFeaturedDeal = _mapper.Map<Models.FeaturedDeal>(featuredDeal);

            _dbContext.FeaturedDeals.Remove(mappedFeaturedDeal);

            _dbContext.SaveChanges();
        }

        public async Task<bool> FeaturedDealExists(int featuredDealId)
        {
            return await _dbContext.FeaturedDeals.AnyAsync(x => x.Id == featuredDealId);
        }

        public async Task<List<FeaturedDeal>> GetAllFeaturedDealsAsync()
        {
            var featuredDeals = await _dbContext.FeaturedDeals.ToListAsync();

            return _mapper.Map<List<FeaturedDeal>>(featuredDeals);
        }

        public async Task<FeaturedDeal> GetFeaturedDealByIdAsync(int featuredDealId)
        {
            var featuredDeal = await _dbContext.FeaturedDeals
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == featuredDealId);

            return _mapper.Map<FeaturedDeal>(featuredDeal);
        }

        public async Task UpdateFeaturedDealAsync(FeaturedDeal featuredDeal)
        {
            if (!await FeaturedDealExists(featuredDeal.Id))
            {
                throw new Exception("FeaturedDeal not found");
            }

            var mappedFeaturedDeal = _mapper.Map<Models.FeaturedDeal>(featuredDeal);

            _dbContext.FeaturedDeals.Update(mappedFeaturedDeal);

            _dbContext.SaveChanges();
        }
    }
}
