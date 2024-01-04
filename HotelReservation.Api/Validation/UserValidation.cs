using FluentValidation;
using HotelReservation.Api.Models;

namespace HotelReservation.Api.Validation
{
    public class UserValidation : AbstractValidator<UserDTO>
    {
        public UserValidation()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .NotEmpty().WithMessage("Username should not be empty");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty().WithMessage("Password should not be empty");

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty().WithMessage("Email should not be empty")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(user => user.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => role.ToLower() == "user" || role.ToLower() == "admin")
                .WithMessage("Role must be either user or admin");
        }
    }
}
