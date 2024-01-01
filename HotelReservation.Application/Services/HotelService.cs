using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task AddHotelAsync(Hotel hotel)
        {
            await _hotelRepository.AddHotelAsync(hotel);
        }

        public async Task DeleteHotelAsync(int hotelId)
        {
            await _hotelRepository.DeleteHotelAsync(hotelId);
        }

        public async Task<List<Hotel>> GetAllHotelsAsync()
        {
            return await _hotelRepository.GetAllHotelsAsync();
        }

        public async Task<Hotel> GetHotelByIdAsync(int hotelId)
        {
            return await _hotelRepository.GetHotelByIdAsync(hotelId);
        }

        public async Task<bool> HotelExists(int hotelId)
        {
            return await _hotelRepository.HotelExists(hotelId);
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            await _hotelRepository.UpdateHotelAsync(hotel);
        }
    }
}
