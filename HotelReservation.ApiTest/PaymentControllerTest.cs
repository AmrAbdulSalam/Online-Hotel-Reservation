using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Enums;
using HotelReservation.Domain.Models;

namespace HotelReservation.ApiTest
{
    public class PaymentControllerTest : IntegrationTest
    {
        private readonly PaymentDTO _payment;

        public PaymentControllerTest() : base()
        {
            _payment = new PaymentDTO
            {
                PaymentStatus = PaymentStatusEnum.Pending,
                PaymentMethod = PaymentMethodEnum.Cash,
                Amount = 50,
                ReservationId = 21
            };
        }

        [Fact]
        public async Task GetAllPayments()
        {
            var unauthrizedResponse = await _client.GetAsync("api/payments");

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.GetAsync("api/payments");

            await AuthenticateAsync(_admin);
            var okResponse = await _client.GetAsync("api/payments");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetPaymentById()
        {
            var paymentId = await GetPaymentId();

            var unauthrizedResponse = await _client.GetAsync($"api/payments/{paymentId}");

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.GetAsync($"api/payments/{paymentId}");

            await AuthenticateAsync(_admin);
            var okResponse = await _client.GetAsync($"api/payments/{paymentId}");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreatePayment()
        {
            var unauthrizedResponse = await _client.PostAsJsonAsync("api/payments", _payment);

            await AuthenticateAsync(_admin);
            var forbiddenResponse = await _client.PostAsJsonAsync("api/payments", _payment);

            await AuthenticateAsync(_user);
            var createdResponse = await _client.PostAsJsonAsync("api/payments", _payment);

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdatePayment()
        {
            var updatedPayment = new PaymentDTO
            {
                PaymentStatus = PaymentStatusEnum.Failed,
                PaymentMethod = PaymentMethodEnum.Visa,
                Amount = 75,
                ReservationId = 21
            };

            var paymentId = await GetPaymentId();

            var unauthrizedResponse = await _client.PutAsJsonAsync($"api/payments/{paymentId}", updatedPayment);

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.PutAsJsonAsync($"api/payments/{paymentId}", updatedPayment);

            await AuthenticateAsync(_admin);
            var noContentResponse = await _client.PutAsJsonAsync($"api/payments/{paymentId}", updatedPayment);

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            noContentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeletePayment()
        {
            var paymentId = await GetPaymentId();

            var unauthrizedResponse = await _client.DeleteAsync($"api/payments/{paymentId}");

            await AuthenticateAsync(_user);
            var forbiddenResponse = await _client.DeleteAsync($"api/payments/{paymentId}");

            await AuthenticateAsync(_admin);
            var noContentResponse = await _client.DeleteAsync($"api/payments/{paymentId}");

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            noContentResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        private async Task<int> GetPaymentId()
        {
            await AuthenticateAsync(_user);
            var createdResponse = await _client.PostAsJsonAsync("api/payments", _payment);

            var createdPayment = await createdResponse.Content.ReadAsAsync<Payment>();

            _client.DefaultRequestHeaders.Authorization = null;

            return createdPayment.Id;
        }
    }
}
