using HotelReservation.Domain.Models;

namespace HotelReservation.Domain
{
    public interface IFormCreater
    {
        Task<string> CreateFormAsync(Reservation reservation);
    }
}
