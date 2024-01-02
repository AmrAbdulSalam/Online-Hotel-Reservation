﻿using AutoMapper;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserRepository(HotelReservationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var mappedUser = _mapper.Map<Models.User>(user);

            await _dbContext.Users.AddAsync(mappedUser);

            _dbContext.SaveChanges();

            return mappedUser.Id;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);

            var mappedUser = _mapper.Map<Models.User>(user);

            _dbContext.Users.Remove(mappedUser);

            _dbContext.SaveChanges();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _dbContext.Users.ToListAsync();

            return _mapper.Map<List<User>>(users);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);

            return _mapper.Map<User>(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            if (!await UserExists(user.Id))
            {
                throw new Exception("User not found");
            }

            var mappedUser = _mapper.Map<Models.User>(user);

            _dbContext.Users.Update(mappedUser);

            _dbContext.SaveChanges();
        }

        public async Task<bool> UserExists(int userId)
        {
            return await _dbContext.Users.AnyAsync(x => x.Id == userId);
        }
    }
}
