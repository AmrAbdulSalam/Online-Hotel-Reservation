using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface ICityService
    {
        Task<List<City>> GetAllCitiesAsync();

        Task<City> GetCityByIdAsync(int cityId);

        Task AddCityAsync(City city);

        Task UpdateCityAsync(City city);

        Task DeleteCityAsync(int cityId);

        Task<bool> CityExists(int cityId);
    }
}
