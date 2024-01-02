using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.RepositoryInterfaces
{
    public interface ICityRepository
    {
        Task<List<City>> GetAllCitiesAsync();

        Task<City> GetCityByIdAsync(int cityId);

        Task<int> AddCityAsync(City city);

        Task UpdateCityAsync(City city);

        Task DeleteCityAsync(int cityId);

        Task<bool> CityExists(int cityId);
    }
}
