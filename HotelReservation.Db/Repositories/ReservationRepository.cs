using AutoMapper;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ReservationRepository(HotelReservationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> AddReservationAsync(Reservation reservation)
        {
            if (reservation == null)
            {
                throw new ArgumentNullException(nameof(reservation));
            }

            var mappedReservation = _mapper.Map<Models.Reservation>(reservation);

            await _dbContext.Reservations.AddAsync(mappedReservation);

            _dbContext.SaveChanges();

            return mappedReservation.Id;
        }

        public async Task DeleteReservationAsync(int reservationId)
        {
            var reservation = await GetReservationByIdAsync(reservationId);

            if (reservation == null)
            {
                throw new ArgumentNullException();
            }

            var mappedReservation = _mapper.Map<Models.Reservation>(reservation);

            _dbContext.Reservations.Remove(mappedReservation);

            _dbContext.SaveChanges();
        }

        public async Task<List<Reservation>> GetAllReservationsAsync(int pageNumber, int pageSize)
        {
            var reservations = await _dbContext.Reservations
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<Reservation>>(reservations);
        }

        public async Task<Reservation> GetReservationByIdAsync(int reservationId)
        {
            var reservation = await _dbContext.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == reservationId);

            return _mapper.Map<Reservation>(reservation);
        }

        public async Task<bool> IsReservationAvailableAsync(int roomId, DateTime startDate, DateTime endDate)
        {
            var reservations = await _dbContext.Reservations.ToListAsync();

            foreach (var reservation in reservations)
            {
                bool sameRoom = reservation.RoomId == roomId;
                bool overlap = !(endDate <= reservation.CheckIn || startDate >= reservation.CheckOut);

                if (sameRoom && overlap)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> ReservationExists(int reservationId)
        {
            return await _dbContext.Reservations.AnyAsync(x => x.Id == reservationId);
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            if (!await ReservationExists(reservation.Id))
            {
                throw new Exception("Reservation not found");
            }

            var mappedReservation = _mapper.Map<Models.Reservation>(reservation);

            _dbContext.Reservations.Update(mappedReservation);

            _dbContext.SaveChanges();
        }
    }
}
