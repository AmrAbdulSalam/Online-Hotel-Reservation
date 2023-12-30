using HotelReservation.Db.Enums;

namespace HotelReservation.Db.Models
{
    internal class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string PostOffice { get; set; }
        public CurrencyEnum Currency { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public List<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
