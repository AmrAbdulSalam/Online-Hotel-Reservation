using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.ApiTest
{
    public class FeaturedDealControllerTest : IntegrationTest
    {
        private readonly FeaturedDealDTO _featuredDeal;

        public FeaturedDealControllerTest() : base()
        {
            _featuredDeal = new FeaturedDealDTO
            {
                Discount = 0.2,
                Description = "WinterDeals",
                PromoCode = "UITS2024-T1",
                EndDate = DateTime.Now.AddDays(20),
                HotelId = 12
            };
        }

        [Fact]
        public async Task GetAllFeaturedDeals()
        {
            var unauthrizedResponse = await _client.GetAsync("api/featured-deals");

            await AuthenticateAsync(_user);
            var okResponseForUser = await _client.GetAsync("api/featured-deals");

            await AuthenticateAsync(_admin);
            var okResponseForAdmin = await _client.GetAsync("api/featured-deals");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            okResponseForUser.StatusCode.Should().Be(HttpStatusCode.OK);
            okResponseForAdmin.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetFeaturedDealById_ReturnsCorrectResponseCodes()
        {
            int featuredDealId = await GetId();
            var unauthrizedResponse = await _client.GetAsync($"api/featured-deals/{featuredDealId}");

            await AuthenticateAsync(_user);
            var okResponseForUser = await _client.GetAsync($"api/featured-deals/{featuredDealId}");

            await AuthenticateAsync(_admin);
            var okResponseForAdmin = await _client.GetAsync($"api/featured-deals/{featuredDealId}");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            okResponseForUser.StatusCode.Should().Be(HttpStatusCode.OK);
            okResponseForAdmin.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateFeaturedDeal_ReturnsCorrectResponseCodes()
        {
            var unauthrizedResponse = await _client.PostAsJsonAsync("api/featured-deals", _featuredDeal);

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.PostAsJsonAsync("api/featured-deals", _featuredDeal);

            await AuthenticateAsync(_admin);
            var createdResponse = await _client.PostAsJsonAsync("api/featured-deals", _featuredDeal);

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdateFeaturedDeal()
        {
            int featuredDealId = await GetId();
            var unauthrizedResponse = await _client.PutAsJsonAsync($"api/featured-deals/{featuredDealId}", _featuredDeal);

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.PutAsJsonAsync($"api/featured-deals/{featuredDealId}", _featuredDeal);

            await AuthenticateAsync(_admin);
            var noContentResponse = await _client.PutAsJsonAsync($"api/featured-deals/{featuredDealId}", _featuredDeal);

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            noContentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteFeaturedDeal()
        {
            int featuredDealId = await GetId();
            var unauthrizedResponse = await _client.DeleteAsync($"api/featured-deals/{featuredDealId}");

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.DeleteAsync($"api/featured-deals/{featuredDealId}");

            await AuthenticateAsync(_admin);
            var noContentResponse = await _client.DeleteAsync($"api/featured-deals/{featuredDealId}");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            noContentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        private async Task<int> GetId()
        {
            await AuthenticateAsync(_admin);
            var createdResponse = await _client.PostAsJsonAsync("api/featured-deals", _featuredDeal);

            var createdFeaturedDeal = await createdResponse.Content.ReadAsAsync<FeaturedDeal>();

            _client.DefaultRequestHeaders.Authorization = null;

            return createdFeaturedDeal.Id;
        }
    }
}
