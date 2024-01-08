using HotelReservation.Application.Services;
using HotelReservation.Domain.Enums;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Moq;

namespace HotelReservation.Test
{
    public class RoomServiceTest
    {
        private readonly Room _room;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;

        public RoomServiceTest()
        {
            _room = new Room
            {
                Id = 1,
                Available = true,
                RoomNumber = "101",
                Type = RoomEnum.Luxury,
                Image = "room_image.jpg",
                AdultCapacity = 2,
                ChildrenCapacity = 1,
                PricePerNight = 100.0,
                HotelId = 1
            };

            _roomRepositoryMock = new Mock<IRoomRepository>();
        }

        [Fact]
        public async Task AddRoomAsync_ValidRoom_ReturnsRoomId()
        {
            _roomRepositoryMock.Setup(repo => repo.AddRoomAsync(_room)).ReturnsAsync(_room.Id);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            var result = await roomService.AddRoomAsync(_room);

            Assert.Equal(_room.Id, result);
        }

        [Fact]
        public async Task GetRoomByIdAsync_ExistingRoomId_ReturnsRoom()
        {
            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(_room.Id)).ReturnsAsync(_room);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            var result = await roomService.GetRoomByIdAsync(_room.Id);

            Assert.Equal(_room, result);
        }

        [Fact]
        public async Task GetRoomByIdAsync_NonExistingRoomId_ReturnsNull()
        {
            int nonExistingRoomId = 999;
            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(nonExistingRoomId)).ReturnsAsync((Room)null);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            var result = await roomService.GetRoomByIdAsync(nonExistingRoomId);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteRoomAsync_ExistingRoomId_DeletesRoom()
        {
            _roomRepositoryMock.Setup(repo => repo.DeleteRoomAsync(_room.Id)).Returns(Task.CompletedTask);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            await roomService.DeleteRoomAsync(_room.Id);

            _roomRepositoryMock.Verify(repo => repo.DeleteRoomAsync(_room.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteRoomAsync_NonExistingRoomId_ThrowsException()
        {
            int nonExistingRoomId = 999;
            _roomRepositoryMock.Setup(repo => repo.DeleteRoomAsync(nonExistingRoomId)).ThrowsAsync(new ArgumentNullException());

            var roomService = new RoomService(_roomRepositoryMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await roomService.DeleteRoomAsync(nonExistingRoomId));
        }

        [Fact]
        public async Task UpdateRoomAsync_ValidRoom_ReturnsUpdatedRoom()
        {
            var updatedRoom = new Room
            {
                Id = 1,
                Available = true,
                RoomNumber = "101",
                Type = RoomEnum.Suite,
                Image = "room_image.jpg",
                AdultCapacity = 5,
                ChildrenCapacity = 0,
                PricePerNight = 300.0,
                HotelId = 1
            };

            _roomRepositoryMock.Setup(repo => repo.UpdateRoomAsync(_room)).Returns(Task.CompletedTask);
            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(updatedRoom.Id)).ReturnsAsync(updatedRoom);

            var roomService = new RoomService(_roomRepositoryMock.Object);

            await roomService.UpdateRoomAsync(_room);

            _roomRepositoryMock.Verify(repo => repo.UpdateRoomAsync(_room), Times.Once);

            var result = await roomService.GetRoomByIdAsync(updatedRoom.Id);

            Assert.Equal(updatedRoom.Available, result.Available);
            Assert.Equal(updatedRoom.RoomNumber, result.RoomNumber);
            Assert.Equal(updatedRoom.Type, result.Type);
            Assert.Equal(updatedRoom.Image, result.Image);
            Assert.Equal(updatedRoom.AdultCapacity, result.AdultCapacity);
            Assert.Equal(updatedRoom.ChildrenCapacity, result.ChildrenCapacity);
            Assert.Equal(updatedRoom.PricePerNight, result.PricePerNight);
            Assert.Equal(updatedRoom.HotelId, result.HotelId);
        }

        [Fact]
        public async Task UpdateRoomAsync_NonExistingRoom_ThrowsException()
        {
            int nonExistingRoomId = 999;
            var updatedRoom = new Room
            {
                Id = nonExistingRoomId,
                Available = true,
                RoomNumber = "101",
                Type = RoomEnum.Suite,
                Image = "room_image.jpg",
                AdultCapacity = 5,
                ChildrenCapacity = 0,
                PricePerNight = 300.0,
                HotelId = 1
            };

            _roomRepositoryMock.Setup(repo => repo.UpdateRoomAsync(updatedRoom)).ThrowsAsync(new Exception());

            var roomService = new RoomService(_roomRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await roomService.UpdateRoomAsync(updatedRoom));
        }

        [Fact]
        public void FeaturedDealByRoomId_ExistingRoomId_ReturnsFeaturedDeal()
        {
            var featuredDeal = new FeaturedDeal
            {
                Id = 1,
                Discount = 0.2,
                Description = "Summer Deal",
                PromoCode = "Summer",
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            _roomRepositoryMock.Setup(repo => repo.FeaturedDealByRoomId(_room.Id)).Returns(featuredDeal);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            var result = roomService.FeaturedDealByRoomId(_room.Id);

            Assert.NotNull(result);
            Assert.Equal(featuredDeal, result);
        }

        [Fact]
        public async Task GetAllRoomsAsync_ReturnsListOfRooms()
        {
            var rooms = new List<Room> { _room };
            _roomRepositoryMock.Setup(repo => repo.GetAllRoomsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(rooms);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            var result = await roomService.GetAllRoomsAsync(1, 10);

            Assert.Equal(rooms, result);
        }

        [Fact]
        public async Task RoomExists_ExistingRoomId_ReturnsTrue()
        {
            _roomRepositoryMock.Setup(repo => repo.RoomExists(_room.Id)).ReturnsAsync(true);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            var result = await roomService.RoomExists(_room.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task RoomExists_NonExistingRoomId_ReturnsFalse()
        {
            int nonExistingId = 999;
            _roomRepositoryMock.Setup(repo => repo.RoomExists(nonExistingId)).ReturnsAsync(false);
            var roomService = new RoomService(_roomRepositoryMock.Object);

            var result = await roomService.RoomExists(nonExistingId);

            Assert.False(result);
        }
    }
}
