using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Application.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task AddCityAsync(City city)
        {
            await _cityRepository.AddCityAsync(city);
        }

        public async Task<bool> CityExists(int cityId)
        {
            return await _cityRepository.CityExists(cityId);
        }

        public async Task DeleteCityAsync(int cityId)
        {
            await _cityRepository.DeleteCityAsync(cityId);
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _cityRepository.GetAllCitiesAsync();
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            return await _cityRepository.GetCityByIdAsync(cityId);
        }

        public async Task UpdateCityAsync(City city)
        {
            await _cityRepository.UpdateCityAsync(city);
        }
    }
}
