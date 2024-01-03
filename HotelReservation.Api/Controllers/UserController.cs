using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
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

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsersAsync()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [HttpGet("{userId}" , Name = "GetUserById")]
        public async Task<ActionResult<User>> GetUserByIdAsync(int userId)
        {
            var userExists = await _userService.UserExists(userId);

            if (!userExists)
            {
                return NotFound($"User with ID {userId} not found");
            }

            return Ok(await _userService.GetUserByIdAsync(userId));
        }

        [HttpPost]
        public async Task<ActionResult<User>> GetUserByIdAsync(UserDTO newUser)
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

        [HttpDelete("{userId}")]
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

        [HttpPut("{userId}")]
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
