using HotelReservation.Domain.Enums;

namespace HotelReservation.Domain.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string PostOffice { get; set; }
        public CurrencyEnum Currency { get; set; }
        public DateTime CreationDate { get; private set; }
        public DateTime ModificationDate { get; private set; }

        public City()
        {
            CreationDate = DateTime.UtcNow;
            ModificationDate = DateTime.UtcNow;
        }

        public void UpdateModificationDate()
        {
            ModificationDate = DateTime.UtcNow;
        }
    }
}
