using HotelReservation.Domain.Enums;

namespace HotelReservation.Domain.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public DateTime PaymentDate { get; private set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public double Amount { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public Payment()
        {
            PaymentDate = DateTime.UtcNow;
        }

        public void UpdatePaymentDate()
        {
            PaymentDate = DateTime.UtcNow;
        }
    }
}
