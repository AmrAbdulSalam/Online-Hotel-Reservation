using HotelReservation.Domain.Enums;

namespace HotelReservation.Api.Models
{
    public class RoomDTO
    {
        public bool Available { get; set; }
        public string RoomNumber { get; set; }
        public RoomEnum Type { get; set; }
        public IFormFile Image { get; set; }
        public int AdultCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public double PricePerNight { get; set; }
        public int HotelId { get; set; }
    }
}
