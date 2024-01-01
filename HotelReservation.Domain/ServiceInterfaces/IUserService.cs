using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();

        Task<User> GetUserByIdAsync(int userId);

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(int userId);

        Task<bool> UserExists(int userId);
    }
}
