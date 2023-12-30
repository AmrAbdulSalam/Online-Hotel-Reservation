using HotelReservation.Domain.Enums;

namespace HotelReservation.Domain.Models
{
    internal class Room
    {
        public int Id { get; set; }
        public bool Available { get; set; }
        public string RoomNumber { get; set; }
        public RoomEnum Type { get; set; }
        public string Image { get; set; }
        public int AdultCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public double PricePerNight { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
