using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using HotelReservation.Api.Models;
using System.Net.Http.Headers;
using HotelReservation.Domain.Models;
using HotelReservation.ApiTest.Responses;

namespace HotelReservation.ApiTest
{
    public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly User _user;
        protected readonly User _admin;
        protected readonly HttpClient _client;

        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>();

            _user = new User()
            {
                Username = "amr",
                Email = "amrsalam@hotmail.com",
                Password = "amr123",
                Role = "user"
            };

            _admin = new User()
            {
                Username = "admin",
                Email = "admin@gmail.com",
                Password = "admin",
                Role = "admin"
            };

            _client = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync(User user)
        {
            var token = await GetTokenAsync(user);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetTokenAsync(User user)
        {
            if (user == null)
            {
                return null;
            }

            var response = await _client.PostAsJsonAsync("/api/authenticate", new LoginUserDTO()
            {
                Username = user.Username,
                Password = user.Password
            });

            var token = await response.Content.ReadAsAsync<TokenResponse>();

            return token.Data;
        }
    }
}
