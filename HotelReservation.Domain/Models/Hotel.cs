
namespace HotelReservation.Domain.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double StarRate { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public DateTime CreationDate { get; private set; }
        public DateTime ModificationDate { get; private set; }
        public int CityId { get; set; }
        public City City { get; set; }

        public Hotel()
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
