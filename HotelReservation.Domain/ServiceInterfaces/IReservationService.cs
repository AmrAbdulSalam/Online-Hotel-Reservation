using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetAllReservationsAsync();

        Task<Reservation> GetReservationByIdAsync(int reservationId);

        Task<int> AddReservationAsync(Reservation reservation);

        Task UpdateReservationAsync(Reservation reservation);

        Task DeleteReservationAsync(int reservationId);

        Task<bool> ReservationExists(int reservationId);

        Task<bool> IsReservationAvailableAsync(int roomId, DateTime startDate, DateTime endDate);
    }
}
