﻿using HotelReservation.Domain.Models;
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

        public async Task<int> AddCityAsync(City city)
        {
            return await _cityRepository.AddCityAsync(city);
        }

        public async Task<bool> CityExists(int cityId)
        {
            return await _cityRepository.CityExists(cityId);
        }

        public async Task DeleteCityAsync(int cityId)
        {
            await _cityRepository.DeleteCityAsync(cityId);
        }

        public async Task<List<City>> GetAllCitiesAsync(int pageNumber, int pageSize)
        {
            return await _cityRepository.GetAllCitiesAsync(pageNumber, pageSize);
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            return await _cityRepository.GetCityByIdAsync(cityId);
        }

        public List<City> MostVistedCities()
        {
            return _cityRepository.MostVistedCities();
        }

        public async Task UpdateCityAsync(City city)
        {
            await _cityRepository.UpdateCityAsync(city);
        }
    }
}
