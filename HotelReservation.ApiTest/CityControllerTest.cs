using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HotelReservation.Api.Models.CityModel;
using HotelReservation.Domain.Enums;
using HotelReservation.Domain.Models;

namespace HotelReservation.ApiTest
{
    public class CityControllerTest : IntegrationTest
    {
        private readonly CityDTO _city;

        public CityControllerTest() : base()
        {
            _city = new CityDTO
            {
                Name = "Budapest",
                Country = "Hungary",
                PostOffice = "Bud-Post",
                Currency = CurrencyEnum.EURO
            };
        }

        [Fact]
        public async Task GetAllCities()
        {
            var unauthrizedResponse = await _client.GetAsync("api/cities");

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.GetAsync("api/cities");

            await AuthenticateAsync(_admin);
            var okResponse = await _client.GetAsync("api/cities");

            var notFoundResponse = await _client.GetAsync("api/cities/5555");
            var objectBadRequentResponse = await notFoundResponse.Content.ReadAsStringAsync();

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectBadRequentResponse.Should().BeOfType<string>();
        }

        [Fact]
        public async Task CreateCity()
        {
            var unauthrizedResponse = await _client.PostAsJsonAsync("api/cities",_city);

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.PostAsJsonAsync("api/cities",_city);

            await AuthenticateAsync(_admin);
            var createdResponse = await _client.PostAsJsonAsync("api/cities",_city);

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdateCity()
        {
            var updatedCity = new CityDTO
            {
                Name = "updated",
                Country = "Hupdated",
                PostOffice = "Bud-Post",
                Currency = CurrencyEnum.EURO
            };

            int _id = await GetId();

            var unauthrizedResponse = await _client.PutAsJsonAsync($"api/cities/{_id}", updatedCity);

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.PutAsJsonAsync($"api/cities/{_id}", updatedCity);

            await AuthenticateAsync(_admin);
            var noContentResponse = await _client.PutAsJsonAsync($"api/cities/{_id}", updatedCity);

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            noContentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteCity()
        {
            int _id = await GetId();

            var unauthrizedResponse = await _client.DeleteAsync($"api/cities/{_id}");

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.DeleteAsync($"api/cities/{_id}");

            await AuthenticateAsync(_admin);
            var noContentResponse = await _client.DeleteAsync($"api/cities/{_id}");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            noContentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task MostVisitedCities()
        {
            var unauthrizedResponse = await _client.GetAsync("api/cities/most-visited-cities");

            await AuthenticateAsync(_user);
            var okResponseForUser = await _client.GetAsync("api/cities/most-visited-cities");

            await AuthenticateAsync(_admin);
            var okResponseForAdmin = await _client.GetAsync("api/cities/most-visited-cities");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            okResponseForUser.StatusCode.Should().Be(HttpStatusCode.OK);
            okResponseForAdmin.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task<int> GetId()
        {
            await AuthenticateAsync(_admin);
            var createdResponse = await _client.PostAsJsonAsync("api/cities", _city);

            var createdCity = await createdResponse.Content.ReadAsAsync<City>();

            _client.DefaultRequestHeaders.Authorization = null;

            return createdCity.Id;
        }
    }
}
