﻿using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface ICityService
    {
        Task<List<City>> GetAllCitiesAsync(int pageNumber, int pageSize);

        Task<City> GetCityByIdAsync(int cityId);

        Task<int> AddCityAsync(City city);

        Task UpdateCityAsync(City city);

        Task DeleteCityAsync(int cityId);

        Task<bool> CityExists(int cityId);

        List<City> MostVistedCities();
    }
}
