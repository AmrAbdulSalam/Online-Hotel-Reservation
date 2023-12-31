using AutoMapper;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    internal class ReservationRepository : IReservationRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ReservationRepository(HotelReservationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            if (reservation == null)
            {
                throw new ArgumentNullException(nameof(reservation));
            }

            var mappedReservation = _mapper.Map<Models.Reservation>(reservation);

            await _dbContext.Reservations.AddAsync(mappedReservation);

            _dbContext.SaveChanges();
        }

        public async Task DeleteReservationAsync(int reservationId)
        {
            var reservation = await GetReservationByIdAsync(reservationId);

            var mappedReservation = _mapper.Map<Models.Reservation>(reservation);

            _dbContext.Reservations.Remove(mappedReservation);

            _dbContext.SaveChanges();
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            var reservations = _dbContext.Reservations.ToListAsync();

            return _mapper.Map<List<Reservation>>(reservations);
        }

        public async Task<Reservation> GetReservationByIdAsync(int reservationId)
        {
            var reservation = await _dbContext.Reservations.FindAsync(reservationId);

            return _mapper.Map<Reservation>(reservation);
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
