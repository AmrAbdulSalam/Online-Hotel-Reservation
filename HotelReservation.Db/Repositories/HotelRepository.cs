﻿using AutoMapper;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        public HotelRepository(HotelReservationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> AddHotelAsync(Hotel hotel)
        {
            if (hotel == null)
            {
                throw new ArgumentNullException(nameof(hotel));
            }

            var mappedHotel = _mapper.Map<Models.Hotel>(hotel);

            await _dbContext.Hotels.AddAsync(mappedHotel);

            _dbContext.SaveChanges();

            return mappedHotel.Id;
        }

        public async Task DeleteHotelAsync(int hotelId)
        {
            var hotel = await GetHotelByIdAsync(hotelId);

            if (hotel == null)
            {
                throw new ArgumentNullException();
            }

            var mappedHotel = _mapper.Map<Models.Hotel>(hotel);

            _dbContext.Hotels.Remove(mappedHotel);

            _dbContext.SaveChanges();
        }

        public async Task<List<Hotel>> GetAllHotelsAsync(int pageNumber, int pageSize)
        {
            var hotels = await _dbContext.Hotels
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<Hotel>>(hotels);
        }

        public async Task<Hotel> GetHotelByIdAsync(int hotelId)
        {
            var hotel = await _dbContext.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == hotelId);

            return _mapper.Map<Hotel>(hotel);
        }

        public async Task<bool> HotelExists(int hotelId)
        {
            return await _dbContext.Hotels.AnyAsync(x => x.Id == hotelId);
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            if (!await HotelExists(hotel.Id))
            {
                throw new Exception("Hotel not found");
            }

            var mappedHotel = _mapper.Map<Models.Hotel>(hotel);

            _dbContext.Hotels.Update(mappedHotel);

            _dbContext.SaveChanges();
        }
    }
}
