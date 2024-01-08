using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain;
using Moq;
using HotelReservation.Application.Services;

namespace HotelReservation.Test
{
    public class UserServiceTest
    {
        private readonly User _user;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEncryptionService> _encryptionServiceMock;

        public UserServiceTest()
        {
            _user = new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123", 
                Role = "User",
                Email = "test@gmail.com"
            };

            _userRepositoryMock = new Mock<IUserRepository>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
        }

        [Fact]
        public async Task AddUserAsync_ValidUser_ReturnsUserId()
        {
            _userRepositoryMock.Setup(repo => repo.AddUserAsync(_user)).ReturnsAsync(_user.Id);
            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            var result = await userService.AddUserAsync(_user);

            Assert.Equal(_user.Id, result);
        }

        [Fact]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(_user.Username)).Returns(_user);
            _encryptionServiceMock.Setup(service => service.Encrypt(_user.Password)).Returns(_user.Password);

            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);
            var result = userService.Authenticate(_user.Username, _user.Password);

            Assert.NotNull(result);
            Assert.Equal(_user, result);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUserId_DeletesUser()
        {
            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(_user.Id)).Returns(Task.CompletedTask);
            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            await userService.DeleteUserAsync(_user.Id);

            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(_user.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingUserId_ThrowsException()
        {
            int nonExistingUserId = 99999;
            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(nonExistingUserId)).ThrowsAsync(new ArgumentNullException());

            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userService.DeleteUserAsync(nonExistingUserId));
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUsers()
        {
            var users = new List<User> { _user };
            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(users);
            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            var result = await userService.GetAllUsersAsync(1, 10);

            Assert.Equal(users, result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUserId_ReturnsUser()
        {
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(_user.Id)).ReturnsAsync(_user);
            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            var result = await userService.GetUserByIdAsync(_user.Id);
            Assert.Equal(_user, result);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUserId_ReturnsNull()
        {
            int nonExistingUserId = 999;
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(nonExistingUserId)).ReturnsAsync((User)null);
            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            var result = await userService.GetUserByIdAsync(nonExistingUserId);

            Assert.Null(result);
        }

        [Fact]
        public void RecentlyVisitedHotels_ExistingUserId_ReturnsListOfHotels()
        {
            var hotels = new List<Hotel>();
            _userRepositoryMock.Setup(repo => repo.RecentlyVisitedHotels(_user.Id)).Returns(hotels);

            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            var result = userService.RecentlyVisitedHotels(_user.Id);

            Assert.Equal(hotels, result);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUser_ReturnsUpdatedUser()
        {
            var updatedUser = new User
            {
                Id = _user.Id,
                Username = "testuser",
                Password = "password123",
                Role = "User",
                Email = "test@gmail.com"
            };

            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(_user)).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(_user.Id)).ReturnsAsync(updatedUser);

            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            await userService.UpdateUserAsync(_user);
            var result = await userService.GetUserByIdAsync(_user.Id);

            Assert.Equal(updatedUser.Username, result.Username);
            Assert.Equal(updatedUser.Password, result.Password);
            Assert.Equal(updatedUser.Role, result.Role);
            Assert.Equal(updatedUser.Email, result.Email);
        }

        [Fact]
        public async Task UserExists_ExistingUserId_ReturnsTrue()
        {
            _userRepositoryMock.Setup(repo => repo.UserExists(_user.Id)).ReturnsAsync(true);
            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            var result = await userService.UserExists(_user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task UserExists_NonExistingUserId_ReturnsFalse()
        {
            int nonExistingUserId = 999;
            _userRepositoryMock.Setup(repo => repo.UserExists(nonExistingUserId)).ReturnsAsync(false);
            var userService = new UserService(_userRepositoryMock.Object, _encryptionServiceMock.Object);

            var result = await userService.UserExists(nonExistingUserId);

            Assert.False(result);
        }
    }
}
