﻿using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface IHotelService
    {
        Task<List<Hotel>> GetAllHotelsAsync(int pageNumber, int pageSize);

        Task<Hotel> GetHotelByIdAsync(int hotelId);

        Task<int> AddHotelAsync(Hotel hotel);

        Task UpdateHotelAsync(Hotel hotel);

        Task DeleteHotelAsync(int hotelId);

        Task<bool> HotelExists(int hotelId);
    }
}
