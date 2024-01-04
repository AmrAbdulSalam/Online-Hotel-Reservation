using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Api.Validation
{
    public class HotelValidation : AbstractValidator<HotelDTO>
    {
        public HotelValidation(ICityService cityService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty().WithMessage("Name should not be empty")
                .MaximumLength(20).WithMessage("Name maximum length 20 characters");

            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty().WithMessage("Description should not be empty")
                .MaximumLength(500).WithMessage("Name maximum length 500 characters");

            RuleFor(x => x.StarRate)
                .NotNull()
                .NotEmpty().WithMessage("StarRate should not be empty")
                .InclusiveBetween(0, 7).WithMessage("Star rating must be between 0 and 5");

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("CityId is required")
                .GreaterThan(0).WithMessage("CityId should be greater than 0")
                .MustAsync(async (cityId, cancellation) => await cityService.CityExists(cityId))
                .WithMessage("CityId does not exist");
        }
    }
}
