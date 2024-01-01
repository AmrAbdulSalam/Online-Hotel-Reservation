using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetAllReservationsAsync();

        Task<Reservation> GetReservationByIdAsync(int reservationId);

        Task AddReservationAsync(Reservation reservation);

        Task UpdateReservationAsync(Reservation reservation);

        Task DeleteReservationAsync(int reservationId);

        Task<bool> ReservationExists(int reservationId);
    }
}
