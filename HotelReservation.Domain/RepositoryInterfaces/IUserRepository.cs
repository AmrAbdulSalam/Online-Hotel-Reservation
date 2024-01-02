using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();

        Task<User> GetUserByIdAsync(int userId);

        Task<int> AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(int userId);

        Task<bool> UserExists(int userId);
    }
}
