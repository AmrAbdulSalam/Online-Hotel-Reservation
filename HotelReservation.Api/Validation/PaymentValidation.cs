using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Api.Validation
{
    public class PaymentValidation : AbstractValidator<PaymentDTO>
    {
        public PaymentValidation(IReservationService reservationService)
        {
            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid PaymentMethod Type");

            RuleFor(x => x.PaymentStatus)
                .IsInEnum().WithMessage("Invalid PaymentStatus Type");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("Amount should be greater than 0");

            RuleFor(x => x.ReservationId)
                .NotEmpty().WithMessage("ReservationId is required")
                .GreaterThan(0).WithMessage("ReservationId should be greater than 0")
                .MustAsync(async (reservationId, cancellation) => await reservationService.ReservationExists(reservationId))
                .WithMessage("ReservationId does not exist");
        }
    }
}
