
namespace HotelReservation.Domain.Models
{
    internal class FeaturedDeal
    {
        public int Id { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public string PromoCode { get; set; }
        public DateTime EndDate { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
