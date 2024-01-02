using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface IRoomService
    {
        Task<List<Room>> GetAllRoomsAsync();

        Task<Room> GetRoomByIdAsync(int roomId);

        Task<int> AddRoomAsync(Room room);

        Task UpdateRoomAsync(Room room);

        Task DeleteRoomAsync(int roomId);

        Task<bool> RoomExists(int roomId);
    }
}
