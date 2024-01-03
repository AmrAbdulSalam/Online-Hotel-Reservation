using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Api.Validation
{
    public class FeaturedDealValidation : AbstractValidator<FeaturedDealDTO>
    {
        public FeaturedDealValidation(IHotelService hotelService)
        {
            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty().WithMessage("Description should not be empty")
                .MaximumLength(150).WithMessage("Description maximum length 150 characters");

            RuleFor(x => x.EndDate)
                .NotNull()
                .NotEmpty().WithMessage("EndDate should not be empty")
                .GreaterThan(DateTime.Now).WithMessage("EndDate should be a future date");

            RuleFor(x => x.HotelId)
                .NotEmpty().WithMessage("HotelId is required")
                .GreaterThan(0).WithMessage("HotelId should be greater than 0")
                .MustAsync(async (HotelId, cancellation) => await hotelService.HotelExists(HotelId))
                .WithMessage("HotelId does not exist");
        }
    }
}
