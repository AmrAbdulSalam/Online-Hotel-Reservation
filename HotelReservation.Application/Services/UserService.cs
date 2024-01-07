using HotelReservation.Domain;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;

        public UserService(IUserRepository userRepository, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<int> AddUserAsync(User user)
        {
            return await _userRepository.AddUserAsync(user);
        }

        public User Authenticate(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            var isPasswordValid = _encryptionService.Encrypt(password) == user.Password;

            if (user == null || !isPasswordValid) 
            {
                return null;
            }

            return user;
        }

        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<List<User>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            return await _userRepository.GetAllUsersAsync(pageNumber, pageSize);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public List<Hotel> RecentlyVisitedHotels(int userId)
        {
            return _userRepository.RecentlyVisitedHotels(userId);
        }

        public Task UpdateUserAsync(User user)
        {
            return _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> UserExists(int userId)
        {
            return await _userRepository.UserExists(userId);
        }
    }
}
