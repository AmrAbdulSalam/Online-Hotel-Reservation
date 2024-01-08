using HotelReservation.Application.Services;
using HotelReservation.Domain.Enums;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.ServiceInterfaces;
using Moq;

namespace HotelReservation.Test
{
    public class FeaturedDealServiceTest
    {
        private readonly FeaturedDeal _featuredDeal;
        private readonly Mock<IFeaturedDealRepository> _featuredDealRepositoryMock;

        public FeaturedDealServiceTest()
        {
            _featuredDeal = new FeaturedDeal
            {
                Id = 1,
                Discount = 0.1,
                Description = "Test Deal",
                PromoCode = "TESTCODE",
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            _featuredDealRepositoryMock = new Mock<IFeaturedDealRepository>();
        }

        [Fact]
        public async Task AddFeaturedDealAsync_ValidFeaturedDeal_ReturnsFeaturedDealId()
        {
            _featuredDealRepositoryMock.Setup(repo => repo.AddFeaturedDealAsync(_featuredDeal)).ReturnsAsync(_featuredDeal.Id);
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            var result = await featuredDealService.AddFeaturedDealAsync(_featuredDeal);

            Assert.Equal(_featuredDeal.Id, result);
        }

        [Fact]
        public async Task GetFeaturedDealByIdAsync_ExistingId_ReturnsFeaturedDeal()
        {
            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealByIdAsync(_featuredDeal.Id)).ReturnsAsync(_featuredDeal);
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            var result = await featuredDealService.GetFeaturedDealByIdAsync(_featuredDeal.Id);

            Assert.Equal(_featuredDeal, result);
        }

        [Fact]
        public async Task GetFeaturedDealByIdAsync_NonExistingId_ReturnsNull()
        {
            int nonExistingId = 999;
            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealByIdAsync(nonExistingId)).ReturnsAsync((FeaturedDeal)null);
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            var result = await featuredDealService.GetFeaturedDealByIdAsync(nonExistingId);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteFeaturedDealAsync_ExistingId_DeletesFeaturedDeal()
        {
            _featuredDealRepositoryMock.Setup(repo => repo.DeleteFeaturedDealAsync(_featuredDeal.Id)).Returns(Task.CompletedTask);
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            await featuredDealService.DeleteFeaturedDealAsync(_featuredDeal.Id);

            _featuredDealRepositoryMock.Verify(repo => repo.DeleteFeaturedDealAsync(_featuredDeal.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteFeaturedDealAsync_NonExistingId_ThrowsException()
        {
            int nonExistingId = 999;
            _featuredDealRepositoryMock.Setup(repo => repo.DeleteFeaturedDealAsync(nonExistingId)).ThrowsAsync(new ArgumentNullException());

            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await featuredDealService.DeleteFeaturedDealAsync(nonExistingId));
        }

        [Fact]
        public async Task UpdateFeaturedDealAsync_ValidFeaturedDeal_ReturnsUpdatedFeaturedDeal()
        {
            var updatedFeaturedDeal = new FeaturedDeal
            {
                Id = _featuredDeal.Id,
                Discount = 0.2,
                Description = "Test Deal-Updated",
                PromoCode = "TESTCODE-Updated",
                EndDate = DateTime.UtcNow.AddDays(20)
            };

            _featuredDealRepositoryMock.Setup(repo => repo.UpdateFeaturedDealAsync(updatedFeaturedDeal)).Returns(Task.CompletedTask);
            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealByIdAsync(_featuredDeal.Id)).ReturnsAsync(updatedFeaturedDeal);

            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            await featuredDealService.UpdateFeaturedDealAsync(_featuredDeal);
            var result = await featuredDealService.GetFeaturedDealByIdAsync(_featuredDeal.Id);

            Assert.Equal(updatedFeaturedDeal.Discount, result.Discount);
            Assert.Equal(updatedFeaturedDeal.Description, result.Description);
            Assert.Equal(updatedFeaturedDeal.PromoCode, result.PromoCode);
            Assert.Equal(updatedFeaturedDeal.EndDate, result.EndDate);
        }

        [Fact]
        public async Task UpdateFeaturedDealAsync_NonExistingId_ThrowsException()
        {
            int nonExistingId = 999;
            var updatedDeal = new FeaturedDeal
            {
                Id = nonExistingId,
                Discount = 0.2,
                Description = "Test Deal-Updated",
                PromoCode = "TESTCODE-Updated",
                EndDate = DateTime.UtcNow.AddDays(20)
            };

            _featuredDealRepositoryMock.Setup(repo => repo.UpdateFeaturedDealAsync(updatedDeal)).ThrowsAsync(new Exception());
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await featuredDealService.UpdateFeaturedDealAsync(updatedDeal));
        }

        [Fact]
        public async Task GetAllFeaturedDealsAsync_ReturnsListOfFeaturedDeals()
        {
            var featuredDeals = new List<FeaturedDeal> { _featuredDeal };
            _featuredDealRepositoryMock.Setup(repo => repo.GetAllFeaturedDealsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(featuredDeals);
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            var result = await featuredDealService.GetAllFeaturedDealsAsync(1, 10);

            Assert.Equal(featuredDeals, result);
        }

        [Fact]
        public async Task FeaturedDealExists_ExistingFeaturedDealId_ReturnsTrue()
        {
            _featuredDealRepositoryMock.Setup(repo => repo.FeaturedDealExists(_featuredDeal.Id)).ReturnsAsync(true);
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            var result = await featuredDealService.FeaturedDealExists(_featuredDeal.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task FeaturedDealExists_NonExistingFeaturedDealId_ReturnsFalse()
        {
            int nonExistingId = 999;
            _featuredDealRepositoryMock.Setup(repo => repo.FeaturedDealExists(nonExistingId)).ReturnsAsync(false);
            var featuredDealService = new FeaturedDealService(_featuredDealRepositoryMock.Object);

            var result = await featuredDealService.FeaturedDealExists(nonExistingId);

            Assert.False(result);
        }
    }
}
