using HotelReservation.Domain.Models;

namespace HotelReservation.Domain
{
    public interface ITokenService
    {
        Token GenerateToken(User user);
    }
}
