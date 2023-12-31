using AutoMapper;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    internal class RoomRepository : IRoomRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        internal RoomRepository(HotelReservationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddRoomAsync(Room room)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            var mappedRoom = _mapper.Map<Models.Room>(room);

            await _dbContext.Rooms.AddAsync(mappedRoom);

            _dbContext.SaveChanges();
        }

        public async Task DeleteRoomAsync(int roomId)
        {
            var room = await GetRoomByIdAsync(roomId);

            var mappedRoom = _mapper.Map<Models.Room>(room);

            _dbContext.Rooms.Remove(mappedRoom);

            _dbContext.SaveChanges();
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            var rooms = await _dbContext.Rooms.ToListAsync();

            return _mapper.Map<List<Room>>(rooms);
        }

        public async Task<Room> GetRoomByIdAsync(int roomId)
        {
            var room = await _dbContext.Rooms.FindAsync(roomId);

            return _mapper.Map<Room>(room);
        }

        public async Task<bool> RoomExists(int roomId)
        {
            return await _dbContext.Rooms.AnyAsync(x => x.Id == roomId);
        }

        public async Task UpdateRoomAsync(Room room)
        {
            if (!await RoomExists(room.Id))
            {
                throw new Exception("Room not found");
            }

            var mappedRoom = _mapper.Map<Models.Room>(room);

            _dbContext.Rooms.Update(mappedRoom);

            _dbContext.SaveChanges();
        }
    }
}
