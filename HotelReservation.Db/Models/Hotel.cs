
namespace HotelReservation.Db.Models
{
    internal class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double StarRate { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public List<FeaturedDeal> FeaturedDeals { get; set; } = new List<FeaturedDeal>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
