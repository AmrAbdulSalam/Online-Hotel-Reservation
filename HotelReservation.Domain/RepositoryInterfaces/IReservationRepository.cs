using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.RepositoryInterfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetAllReservationsAsync();

        Task<Reservation> GetReservationByIdAsync(int reservationId);

        Task<int> AddReservationAsync(Reservation reservation);

        Task UpdateReservationAsync(Reservation reservation);

        Task DeleteReservationAsync(int reservationId);

        Task<bool> ReservationExists(int reservationId);
    }
}
