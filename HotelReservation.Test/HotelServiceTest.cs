using HotelReservation.Application.Services;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Moq;

namespace HotelReservation.Test
{
    public class HotelServiceTest
    {
        private readonly Hotel _hotel;
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;

        public HotelServiceTest()
        {
            _hotel = new Hotel
            {
                Id = 1,
                Name = "Test Hotel",
                StarRate = 4,
                Description = "This is a test hotel",
                Address = "123 Test Address",
                Image = "hotel_image.jpg",
                CityId = 1
            };

            _hotelRepositoryMock = new Mock<IHotelRepository>();
        }

        [Fact]
        public async Task AddHotelAsync_ValidHotel_ReturnsHotelId()
        {
            _hotelRepositoryMock.Setup(repo => repo.AddHotelAsync(_hotel)).ReturnsAsync(_hotel.Id);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            var result = await hotelService.AddHotelAsync(_hotel);

            Assert.Equal(_hotel.Id, result);
        }

        [Fact]
        public async Task GetHotelByIdAsync_ExistingId_ReturnsHotel()
        {
            _hotelRepositoryMock.Setup(repo => repo.GetHotelByIdAsync(_hotel.Id)).ReturnsAsync(_hotel);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            var result = await hotelService.GetHotelByIdAsync(_hotel.Id);

            Assert.Equal(_hotel, result);
        }

        [Fact]
        public async Task GetHotelByIdAsync_NonExistingId_ReturnsNull()
        {
            int nonExistingId = 999;
            _hotelRepositoryMock.Setup(repo => repo.GetHotelByIdAsync(nonExistingId)).ReturnsAsync((Hotel)null);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            var result = await hotelService.GetHotelByIdAsync(nonExistingId);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteHotelAsync_ExistingId_DeletesHotel()
        {
            _hotelRepositoryMock.Setup(repo => repo.DeleteHotelAsync(_hotel.Id)).Returns(Task.CompletedTask);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            await hotelService.DeleteHotelAsync(_hotel.Id);

            _hotelRepositoryMock.Verify(repo => repo.DeleteHotelAsync(_hotel.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteHotelAsync_NonExistingId_ThrowsException()
        {
            int nonExistingId = 999;
            _hotelRepositoryMock.Setup(repo => repo.DeleteHotelAsync(nonExistingId)).ThrowsAsync(new ArgumentNullException());

            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await hotelService.DeleteHotelAsync(nonExistingId));
        }

        [Fact]
        public async Task UpdateHotelAsync_ValidHotel_ReturnsUpdatedHotel()
        {
            var updatedHotel = new Hotel
            {
                Id = _hotel.Id,
                Name = "Updated Hotel",
                StarRate = 5.0,
                Description = "This is an updated hotel",
                Address = "456 Updated Address",
                Image = "updated_hotel_image.jpg",
                CityId = 2
            };

            _hotelRepositoryMock.Setup(repo => repo.UpdateHotelAsync(updatedHotel)).Returns(Task.CompletedTask);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            await hotelService.UpdateHotelAsync(updatedHotel);

            _hotelRepositoryMock.Verify(repo => repo.UpdateHotelAsync(updatedHotel), Times.Once);

            Assert.Equal(updatedHotel.Name, updatedHotel.Name);
            Assert.Equal(updatedHotel.StarRate, updatedHotel.StarRate);
            Assert.Equal(updatedHotel.Description, updatedHotel.Description);
            Assert.Equal(updatedHotel.Address, updatedHotel.Address);
            Assert.Equal(updatedHotel.Image, updatedHotel.Image);
            Assert.Equal(updatedHotel.CityId, updatedHotel.CityId);
        }

        [Fact]
        public async Task UpdateHotelAsync_NonExistingId_ThrowsException()
        {
            int nonExistingId = 999;
            var updatedHotel = new Hotel
            {
                Id = nonExistingId,
                Name = "Updated Hotel",
                StarRate = 5.0,
                Description = "This is an updated hotel",
                Address = "456 Updated Address",
                Image = "updated_hotel_image.jpg",
                CityId = 2
            };

            _hotelRepositoryMock.Setup(repo => repo.UpdateHotelAsync(updatedHotel)).ThrowsAsync(new Exception());

            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await hotelService.UpdateHotelAsync(updatedHotel));
        }


        [Fact]
        public async Task GetAllHotelsAsync_ReturnsListOfHotels()
        {
            var hotels = new List<Hotel> { _hotel };
            _hotelRepositoryMock.Setup(repo => repo.GetAllHotelsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(hotels);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            var result = await hotelService.GetAllHotelsAsync(1, 10);

            Assert.Equal(hotels, result);
        }

        [Fact]
        public async Task HotelExists_ExistingHotelId_ReturnsTrue()
        {
            _hotelRepositoryMock.Setup(repo => repo.HotelExists(_hotel.Id)).ReturnsAsync(true);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            var result = await hotelService.HotelExists(_hotel.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task HotelExists_NonExistingHotelId_ReturnsFalse()
        {
            int nonExistingId = 999;
            _hotelRepositoryMock.Setup(repo => repo.HotelExists(nonExistingId)).ReturnsAsync(false);
            var hotelService = new HotelService(_hotelRepositoryMock.Object);

            var result = await hotelService.HotelExists(nonExistingId);

            Assert.False(result);
        }
    }
}

