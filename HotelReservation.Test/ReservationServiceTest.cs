using HotelReservation.Application.Services;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Moq;

namespace HotelReservation.Test
{
    public class ReservationServiceTest
    {
        private readonly Reservation _reservation;
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;

        public ReservationServiceTest()
        {
            _reservation = new Reservation
            {
                Id = 1,
                IsFeaturedDeal = false,
                CheckIn = DateTime.UtcNow.AddDays(5),
                CheckOut = DateTime.UtcNow.AddDays(10),
                ReservationInfoPath = "reservation_info.txt",
                UserId = 1,
                RoomId = 1
            };

            _reservationRepositoryMock = new Mock<IReservationRepository>();
        }

        [Fact]
        public async Task AddReservationAsync_ValidReservation_ReturnsReservationId()
        {
            _reservationRepositoryMock.Setup(repo => repo.AddReservationAsync(_reservation)).ReturnsAsync(_reservation.Id);
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            var result = await reservationService.AddReservationAsync(_reservation);

            Assert.Equal(_reservation.Id, result);
        }

        [Fact]
        public async Task AddReservationAsync_InvalidReservation_ReturnsZero()
        {
            Reservation invalidReservation = null;
            _reservationRepositoryMock.Setup(repo => repo.AddReservationAsync(invalidReservation)).ReturnsAsync(0);
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            var result = await reservationService.AddReservationAsync(invalidReservation);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetReservationByIdAsync_ExistingId_ReturnsReservation()
        {
            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(_reservation.Id)).ReturnsAsync(_reservation);
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            var result = await reservationService.GetReservationByIdAsync(_reservation.Id);

            Assert.Equal(_reservation, result);
        }

        [Fact]
        public async Task GetReservationByIdAsync_NonExistingId_ReturnsNull()
        {
            int nonExistingId = 999;
            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(nonExistingId)).ReturnsAsync((Reservation)null);
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            var result = await reservationService.GetReservationByIdAsync(nonExistingId);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteReservationAsync_ExistingId_DeletesReservation()
        {
            _reservationRepositoryMock.Setup(repo => repo.DeleteReservationAsync(_reservation.Id)).Returns(Task.CompletedTask);
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            await reservationService.DeleteReservationAsync(_reservation.Id);

            _reservationRepositoryMock.Verify(repo => repo.DeleteReservationAsync(_reservation.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteReservationAsync_NonExistingId_ThrowsException()
        {
            int nonExistingId = 999;
            _reservationRepositoryMock.Setup(repo => repo.DeleteReservationAsync(nonExistingId)).ThrowsAsync(new ArgumentNullException());

            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await reservationService.DeleteReservationAsync(nonExistingId));
        }

        [Fact]
        public async Task IsReservationAvailableAsync_ValidDates_ReturnsTrue()
        {
            _reservationRepositoryMock.Setup(repo => repo.IsReservationAvailableAsync(_reservation.RoomId, _reservation.CheckIn, _reservation.CheckOut))
                .ReturnsAsync(true);

            var reservationService = new ReservationService(_reservationRepositoryMock.Object);
            var result = await reservationService.IsReservationAvailableAsync(_reservation.RoomId, _reservation.CheckIn, _reservation.CheckOut);

            Assert.True(result);
        }

        [Fact]
        public async Task IsReservationAvailableAsync_OverlappingDates_ReturnsFalse()
        {
            var overlappingCheckIn = _reservation.CheckIn.AddDays(-3);
            var overlappingCheckOut = _reservation.CheckOut.AddDays(3);

            _reservationRepositoryMock.Setup(repo => repo.IsReservationAvailableAsync(_reservation.RoomId, overlappingCheckIn, overlappingCheckOut))
                .ReturnsAsync(false);

            var reservationService = new ReservationService(_reservationRepositoryMock.Object);
            var result = await reservationService.IsReservationAvailableAsync(_reservation.RoomId, overlappingCheckIn, overlappingCheckOut);

            Assert.False(result);
        }
        [Fact]
        public async Task UpdateReservationAsync_ValidReservation_ReturnsUpdatedReservation()
        {
            var updatedReservation = new Reservation
            {
                Id = _reservation.Id,
                IsFeaturedDeal = false,
                CheckIn = DateTime.UtcNow.AddDays(30),
                CheckOut = DateTime.UtcNow.AddDays(5),
                ReservationInfoPath = "reservation_info-updated.txt",
                UserId = 5,
                RoomId = 10
            };

            _reservationRepositoryMock.Setup(repo => repo.UpdateReservationAsync(_reservation)).Returns(Task.FromResult(updatedReservation));
            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(_reservation.Id)).ReturnsAsync(updatedReservation);

            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            await reservationService.UpdateReservationAsync(updatedReservation);
            var result = await reservationService.GetReservationByIdAsync(_reservation.Id);

            Assert.Equal(updatedReservation, result);
        }

        [Fact]
        public async Task UpdateReservationAsync_NonExistingId_ThrowsException()
        {
            int nonExistingId = 999;
            var updatedReservation = new Reservation
            {
                Id = nonExistingId,
                IsFeaturedDeal = false,
                CheckIn = DateTime.UtcNow.AddDays(30),
                CheckOut = DateTime.UtcNow.AddDays(5),
                ReservationInfoPath = "reservation_info-updated.txt",
                UserId = 5,
                RoomId = 10
            };

            _reservationRepositoryMock.Setup(repo => repo.UpdateReservationAsync(updatedReservation)).ThrowsAsync(new Exception());
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await reservationService.UpdateReservationAsync(updatedReservation));
        }

        [Fact]
        public async Task ReservationExists_ExistingReservationId_ReturnsTrue()
        {
            int existingReservationId = 1; 
            _reservationRepositoryMock.Setup(repo => repo.ReservationExists(existingReservationId)).ReturnsAsync(true);
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            var result = await reservationService.ReservationExists(existingReservationId);

            Assert.True(result);
        }

        [Fact]
        public async Task ReservationExists_NonExistingReservationId_ReturnsFalse()
        {
            int nonExistingReservationId = 99999;
            _reservationRepositoryMock.Setup(repo => repo.ReservationExists(nonExistingReservationId)).ReturnsAsync(false);
            var reservationService = new ReservationService(_reservationRepositoryMock.Object);

            var result = await reservationService.ReservationExists(nonExistingReservationId);

            Assert.False(result);
        }
    }
}
