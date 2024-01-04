using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.RepositoryInterfaces
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAllRoomsAsync(int pageNumber, int pageSize);

        Task<Room> GetRoomByIdAsync(int roomId);

        Task<int> AddRoomAsync(Room room);

        Task UpdateRoomAsync(Room room);

        Task DeleteRoomAsync(int roomId);

        Task<bool> RoomExists(int roomId);
    }
}
