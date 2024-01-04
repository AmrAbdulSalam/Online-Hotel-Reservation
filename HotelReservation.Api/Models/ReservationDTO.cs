
namespace HotelReservation.Api.Models
{
    public class ReservationDTO
    {
        public bool IsFeaturedDeal { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
