﻿using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IValidator<UserDTO> _validator;
        private readonly IEncryptionService _encryptionService;

        public UserController(IUserService userService, IMapper mapper, IValidator<UserDTO> validator, IEncryptionService encryptionService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }


        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>        
        /// <remarks>
        /// Route Defualts:
        ///  
        ///     { 
        ///     Defualt:
        ///         PageNumber=0,
        ///         Count=5
        ///     
        ///     Max:
        ///         Count=10
        ///     }
        ///     
        /// Sample request-1:
        ///     
        ///     GET api/users
        ///     
        /// Sample request-2:
        /// 
        ///     GET api/users?pageNumber=0&pageSize=4
        ///   
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<User>>> GetAllUsersAsync(int pageNumber = 0, int pageSize = 5)
        {
            const int maxPageSize = 10;

            if (pageNumber < 0)
            {
                return BadRequest("Page number should be 0 or greater.");
            }

            if (pageSize <= 0 || pageSize > maxPageSize)
            {
                return BadRequest($"Page size should be between 1 and {maxPageSize}.");
            }

            return Ok(await _userService.GetAllUsersAsync(pageNumber, pageSize));
        }


        /// <summary>
        /// Recently visited hotels by a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>      
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/users/1/recently-visited
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireUserRole")]
        [HttpGet("{userId}/recently-visited")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(List<Hotel>), StatusCodes.Status200OK)]
        public ActionResult<List<Hotel>> RecentlyVisitedHotels(int userId)
        {
            return Ok(_userService.RecentlyVisitedHotels(userId));
        }


        /// <summary>
        /// Get a user by ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>        
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/users/1
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{userId}" , Name = "GetUserById")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> GetUserByIdAsync(int userId)
        {
            var userExists = await _userService.UserExists(userId);

            if (!userExists)
            {
                return NotFound($"User with ID {userId} not found");
            }

            return Ok(await _userService.GetUserByIdAsync(userId));
        }


        /// <summary>
        /// Create and add a new user
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>     
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST api/users
        ///     {
        ///         "Username": "amr",
        ///         "Password": "123456",
        ///         "Email": "amrsalam@hotmail.com",
        ///         "Role": "user"
        ///     }
        ///     
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> AddUserAsync(UserDTO newUser)
        {
            var validationResult = await _validator.ValidateAsync(newUser);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => 
                new
                {
                    Property = error.PropertyName,
                    Error = error.ErrorMessage
                })
                .ToList();

                return BadRequest(new { Errors = errors });
            }

            var mappedUser = _mapper.Map<User>(newUser);

            mappedUser.Password = _encryptionService.Encrypt(newUser.Password);

            mappedUser.Id = await _userService.AddUserAsync(mappedUser);

            return CreatedAtRoute("GetUserById",
                new
                {
                    userId = mappedUser.Id
                },
                mappedUser);
        }


        /// <summary>
        /// Delete a user by ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>        
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE api/users/1
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteUserAsync(int userId)
        {
            var userExists = await _userService.UserExists(userId);

            if (!userExists)
            {
                return NotFound($"User with ID {userId} not found");
            }

            await _userService.DeleteUserAsync(userId);

            return NoContent();
        }


        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>        
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST api/users
        ///     {
        ///         "Username": "amr",
        ///         "Password": "123456",
        ///         "Email": "amrsalam@hotmail.com",
        ///         "Role": "user"
        ///     }
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateUserAsync(int userId , UserDTO updatedUser)
        {
            var userExists = await _userService.UserExists(userId);

            if (!userExists)
            {
                return NotFound($"User with ID {userId} not found");
            }

            var validationResult = await _validator.ValidateAsync(updatedUser);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => 
                new
                {
                    Property = error.PropertyName,
                    Error = error.ErrorMessage
                })
                .ToList();

                return BadRequest(new { Errors = errors });
            }

            var mappedUser = _mapper.Map<User>(updatedUser);

            if (mappedUser.Password != null)
            {
                mappedUser.Password = _encryptionService.Encrypt(updatedUser.Password);
            }

            mappedUser.Id = userId;

            await _userService.UpdateUserAsync(mappedUser);

            return NoContent();
        }
    }
}
