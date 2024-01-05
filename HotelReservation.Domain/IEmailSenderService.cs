using HotelReservation.Domain.Models;

namespace HotelReservation.Domain
{
    public interface IEmailSenderService
    {
        Task<bool> SendConfirmationEmail(Reservation reservation); 
    }
}
