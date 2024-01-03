using FluentValidation;
using HotelReservation.Api.Models.CityModel;

namespace HotelReservation.Api.Validation
{
    public class CityValidation : AbstractValidator<CityDTO>
    {
        public CityValidation()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty().WithMessage("Name should not be empty")
                .MaximumLength(10).WithMessage("Name maximum length 10 characters");

            RuleFor(x => x.PostOffice)
                .NotNull()
                .NotEmpty().WithMessage("PostOffice should not be empty")
                .MaximumLength(10).WithMessage("PostOffice maximum length 10 characters");

            RuleFor(x => x.Country)
                .NotNull()
                .NotEmpty().WithMessage("Country should not be empty")
                .MaximumLength(10).WithMessage("Country maximum length 10 characters");
        }
    }
}
