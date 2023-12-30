using HotelReservation.Domain.Enums;

namespace HotelReservation.Domain.Models
{
    public class Room
    {
        public int Id { get; set; }
        public bool Available { get; set; }
        public string RoomNumber { get; set; }
        public RoomEnum Type { get; set; }
        public string Image { get; set; }
        public int AdultCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public double PricePerNight { get; set; }
        public DateTime CreationDate { get; private set; }
        public DateTime ModificationDate { get; private set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        public Room()
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
