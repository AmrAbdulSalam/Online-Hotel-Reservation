using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.RepositoryInterfaces
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAllRoomsAsync();

        Task<Room> GetRoomByIdAsync(int roomId);

        Task AddRoomAsync(Room room);

        Task UpdateRoomAsync(Room room);

        Task DeleteRoomAsync(int roomId);

        Task<bool> RoomExists(int roomId);
    }
}
