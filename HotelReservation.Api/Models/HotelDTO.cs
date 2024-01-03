
namespace HotelReservation.Api.Models
{
    public class HotelDTO
    {
        public string Name { get; set; }
        public double StarRate { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public IFormFile Image { get; set; }
        public int CityId { get; set; }
    }
}
