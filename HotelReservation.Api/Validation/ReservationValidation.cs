using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Api.Validation
{
    public class ReservationValidation : AbstractValidator<ReservationDTO>
    {
        public ReservationValidation(IUserService userService, IRoomService roomService)
        {
            RuleFor(x => x.CheckIn)
                .NotNull()
                .NotEmpty().WithMessage("CheckIn should not be empty")
                .GreaterThan(DateTime.Now).WithMessage("CheckIn should be a future date");

            RuleFor(x => x.CheckOut)
                .NotNull()
                .NotEmpty().WithMessage("CheckOut should not be empty")
                .GreaterThan(DateTime.Now).WithMessage("CheckOut should be a future date");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required")
                .GreaterThan(0).WithMessage("UserId should be greater than 0")
                .MustAsync(async (userId, cancellation) => await userService.UserExists(userId))
                .WithMessage("UserId does not exist");

            RuleFor(x => x.RoomId)
                .NotEmpty().WithMessage("RoomId is required")
                .GreaterThan(0).WithMessage("RoomId should be greater than 0")
                .MustAsync(async (roomId, cancellation) => await roomService.RoomExists(roomId))
                .WithMessage("RoomId does not exist");
        }
    }
}
