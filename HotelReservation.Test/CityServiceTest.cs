using Moq;
using HotelReservation.Application.Services;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.Enums;

namespace HotelReservation.Test
{
    public class CityServiceTest
    {
        private readonly City _city;
        private readonly Mock<ICityRepository> _cityRepositoryMock;

        public CityServiceTest()
        {
            _city = new City()
            {
                Id = 100,
                Name = "Budapest",
                Country = "Hungary",
                PostOffice = "Budapest-Post",
                Currency = CurrencyEnum.EURO
            };

            _cityRepositoryMock = new Mock<ICityRepository>();
        }

        [Fact]
        public async Task AddCityAsync_ValidCity_ReturnsCityId()
        {
            _cityRepositoryMock.Setup(repo => repo.AddCityAsync(_city)).ReturnsAsync(_city.Id);
            var cityService = new CityService(_cityRepositoryMock.Object);

            var result = await cityService.AddCityAsync(_city);

            Assert.Equal(_city.Id, result);
        }

        [Fact]
        public async Task GetCityByIdAsync_ExistingCityId_ReturnsCity()
        {
            _cityRepositoryMock.Setup(repo => repo.GetCityByIdAsync(_city.Id)).ReturnsAsync(_city);
            var cityService = new CityService(_cityRepositoryMock.Object);

            var result = await cityService.GetCityByIdAsync(_city.Id);

            Assert.Equal(_city, result);
        }

        [Fact]
        public async Task GetCityByIdAsync_NonExistingCityId_ReturnsNull()
        {
            int nonExistingCityId = 99999;
            _cityRepositoryMock.Setup(repo => repo.GetCityByIdAsync(nonExistingCityId)).ReturnsAsync((City)null);
            var cityService = new CityService(_cityRepositoryMock.Object);

            var result = await cityService.GetCityByIdAsync(nonExistingCityId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllCitiesAsync_ReturnsListOfCities()
        {
            var cities = new List<City> { _city };
            _cityRepositoryMock.Setup(repo => repo.GetAllCitiesAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(cities);
            var cityService = new CityService(_cityRepositoryMock.Object);

            var result = await cityService.GetAllCitiesAsync(1, 10);

            Assert.Equal(cities, result);
        }

        [Fact]
        public async Task CityExists_ExistingCityId_ReturnsTrue()
        {
            int existingCityId = 100;
            _cityRepositoryMock.Setup(repo => repo.CityExists(existingCityId)).ReturnsAsync(true);
            var cityService = new CityService(_cityRepositoryMock.Object);

            var result = await cityService.CityExists(existingCityId);

            Assert.True(result);
        }

        [Fact]
        public async Task CityExists_NonExistingCityId_ReturnsFalse()
        {
            int nonExistingCityId = 9999;
            _cityRepositoryMock.Setup(repo => repo.CityExists(nonExistingCityId)).ReturnsAsync(false);
            var cityService = new CityService(_cityRepositoryMock.Object);

            var result = await cityService.CityExists(nonExistingCityId);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCityAsync_ExistingCityId_DeletesCity()
        {
            var existingCityId = _city.Id;
            _cityRepositoryMock.Setup(repo => repo.DeleteCityAsync(existingCityId)).Returns(Task.CompletedTask);
            var cityService = new CityService(_cityRepositoryMock.Object);

            await cityService.DeleteCityAsync(existingCityId);

            _cityRepositoryMock.Verify(repo => repo.DeleteCityAsync(existingCityId), Times.Once);
        }

        [Fact]
        public async Task DeleteCityAsync_NonExistingCityId_ThrowsException()
        {
            int nonExistingCityId = 99999;
            _cityRepositoryMock.Setup(repo => repo.DeleteCityAsync(nonExistingCityId)).ThrowsAsync(new ArgumentNullException());

            var cityService = new CityService(_cityRepositoryMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cityService.DeleteCityAsync(nonExistingCityId));
        }

        [Fact]
        public async Task UpdateCityAsync_ExistingCity_ReturnsUpdatedCity()
        {
            var updatedCity = new City
            {
                Id = _city.Id,
                Name = "UpdatedCityName",
                Country = "UpdatedCountry",
                PostOffice = "UpdatedPostOffice",
                Currency = CurrencyEnum.EURO
            };

            _cityRepositoryMock.Setup(repo => repo.UpdateCityAsync(updatedCity)).Returns(Task.CompletedTask);
            _cityRepositoryMock.Setup(repo => repo.GetCityByIdAsync(_city.Id)).ReturnsAsync(updatedCity);

            var cityService = new CityService(_cityRepositoryMock.Object);

            await cityService.UpdateCityAsync(updatedCity);
            var result = await cityService.GetCityByIdAsync(_city.Id);

            Assert.Equal(updatedCity.Name, result.Name);
            Assert.Equal(updatedCity.Country, result.Country);
            Assert.Equal(updatedCity.PostOffice, result.PostOffice);
            Assert.Equal(updatedCity.Currency, result.Currency);
        }

        [Fact]
        public async Task UpdateCityAsync_NonExistingCity_ThrowsException()
        {
            var nonExistingCityId = 99999;
            var updatedCity = new City
            {
                Id = nonExistingCityId,
                Name = "UpdatedCityName",
                Country = "UpdatedCountry",
                PostOffice = "UpdatedPostOffice",
                Currency = CurrencyEnum.EURO
            };

            _cityRepositoryMock.Setup(repo => repo.UpdateCityAsync(updatedCity)).ThrowsAsync(new Exception());

            var cityService = new CityService(_cityRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await cityService.UpdateCityAsync(updatedCity));
        }
    }
}
