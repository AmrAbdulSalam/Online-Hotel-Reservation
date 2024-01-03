using HotelReservation.Domain.Enums;

namespace HotelReservation.Api.Models.CityModel
{
    public class CityDTO
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string PostOffice { get; set; }
        public CurrencyEnum Currency { get; set; }
    }
}
