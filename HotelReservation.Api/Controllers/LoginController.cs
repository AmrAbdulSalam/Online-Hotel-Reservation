using HotelReservation.Api.Models;
using HotelReservation.Domain;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Api.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public LoginController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult CheckUserCredentials(LoginUserDTO loginUser)
        {
            var user = _userService.Authenticate(loginUser.Username, loginUser.Password);

            if (user == null)
            {
                NotFound($"User with ${loginUser.Username} not found");
            }

            var token = _tokenService.GenerateToken(user);

            if (token.Message == "Faild")
            {
                return BadRequest("Failed to generate token");
            }

            return Ok(token);
        }
    }
}
