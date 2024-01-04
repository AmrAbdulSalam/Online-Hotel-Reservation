using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Api.Validation
{
    public class RoomValidation : AbstractValidator<RoomDTO>
    {
        public RoomValidation(IHotelService hotelService)
        {
            RuleFor(x => x.RoomNumber)
                .NotNull()
                .NotEmpty().WithMessage("RoomNumber should not be empty")
                .MaximumLength(10).WithMessage("RoomNumber maximum length 10 characters");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid Room Type");

            RuleFor(x => x.AdultCapacity)
                .NotNull()
                .NotEmpty().WithMessage("AdultCapacity should not be empty")
                .GreaterThanOrEqualTo(0).WithMessage("AdultCapacity must be greater than 0");

            RuleFor(x => x.ChildrenCapacity)
                .NotNull()
                .NotEmpty().WithMessage("ChildrenCapacity should not be empty")
                .GreaterThanOrEqualTo(0).WithMessage("ChildrenCapacity must be greater than 0");

            RuleFor(x => x.HotelId)
                .NotEmpty().WithMessage("HotelId is required")
                .GreaterThan(0).WithMessage("HotelId should be greater than 0")
                .MustAsync(async (hotelId, cancellation) => await hotelService.HotelExists(hotelId))
                .WithMessage("Hotel does not exist");
        }
    }
}
