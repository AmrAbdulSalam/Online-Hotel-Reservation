using HotelReservation.Db.Enums;

namespace HotelReservation.Db.Models
{
    internal class Payment
    {
        public int Id { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
