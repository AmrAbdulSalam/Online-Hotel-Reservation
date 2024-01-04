using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<int> AddRoomAsync(Room room)
        {
            return await _roomRepository.AddRoomAsync(room);
        }

        public async Task DeleteRoomAsync(int roomId)
        {
            await _roomRepository.DeleteRoomAsync(roomId);
        }

        public async Task<List<Room>> GetAllRoomsAsync(int pageNumber, int pageSize)
        {
            return await _roomRepository.GetAllRoomsAsync(pageNumber, pageSize);
        }

        public async Task<Room> GetRoomByIdAsync(int roomId)
        {
            return await _roomRepository.GetRoomByIdAsync(roomId);
        }

        public async Task<bool> RoomExists(int roomId)
        {
            return await _roomRepository.RoomExists(roomId);
        }

        public async Task UpdateRoomAsync(Room room)
        {
            await _roomRepository.UpdateRoomAsync(room);
        }
    }
}
