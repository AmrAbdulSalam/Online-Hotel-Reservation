using HotelReservation.Domain.Enums;

namespace HotelReservation.Api.Models
{
    public class PaymentDTO
    {
        public PaymentStatusEnum PaymentStatus { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public double Amount { get; set; }
        public int ReservationId { get; set; }
    }
}
