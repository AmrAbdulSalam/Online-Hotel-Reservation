using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CityRepository(HotelReservationDbContext dbContext , IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> AddCityAsync(City city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            var mappedCity = _mapper.Map<Models.City>(city);

            await _dbContext.Cities.AddAsync(mappedCity);

            _dbContext.SaveChanges();

            return mappedCity.Id;
        }
        
        public async Task<bool> CityExists(int cityId)
        {
            return await _dbContext.Cities.AnyAsync(x => x.Id == cityId);
        }

        public async Task DeleteCityAsync(int cityId)
        {
            var city = await GetCityByIdAsync(cityId);

            var mappedCity = _mapper.Map<Models.City>(city);

            _dbContext.Cities.Remove(mappedCity);

            _dbContext.SaveChanges();
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            var cities = await _dbContext.Cities.ToListAsync();

            return _mapper.Map<List<City>>(cities);
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            var city = await _dbContext.Cities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == cityId);

            return _mapper.Map<City>(city);
        }

        public async Task UpdateCityAsync(City city)
        {
            if (!await CityExists(city.Id))
            {
                throw new Exception("City not found");
            }

            var mappedCity = _mapper.Map<Models.City>(city);

            _dbContext.Cities.Update(mappedCity);

            _dbContext.SaveChanges();
        }
    }
}
