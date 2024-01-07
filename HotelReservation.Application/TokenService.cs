using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelReservation.Domain;
using HotelReservation.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HotelReservation.Application
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Token GenerateToken(User user)
        {
            try
            {
                if (_configuration["Token:Key"] == null) throw new ArgumentNullException("Token key is Null");

                var tokenHandle = new JwtSecurityTokenHandler();

                var tKey = Encoding.UTF8.GetBytes(_configuration["Token:Key"]);

                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
                claims.Add(new Claim(ClaimTypes.Role, user.Role.ToLower()));

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tKey), SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandle.CreateToken(tokenDescription);

                return new Token { Data = tokenHandle.WriteToken(token), Message = "Success" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex , "Error while generating token");

                return new Token { Data = null, Message = "Faild" };
            }
        }
    }
}
