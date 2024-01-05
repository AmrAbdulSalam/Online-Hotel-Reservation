
namespace HotelReservation.Domain.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ReferenceceNo { get; private set; }
        public bool IsFeaturedDeal { get; set; }
        public double Price { get; private set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string ReservationInfoPath { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }

        public Reservation()
        {
            ReferenceceNo = GenerateReferenceNumber();
            Price = 0;
        }

        private string GenerateReferenceNumber()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            var referenceNumber = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"REF-{referenceNumber}";
        }

        public void UpdatePrice(double originalPrice, double discount)
        {
            Price = originalPrice - (originalPrice * discount);
        }
    }
}
