using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Api.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IMapper _mapper;
        private readonly IValidator<ReservationDTO> _validator;
        private readonly IRoomService _roomService;

        public ReservationController(IReservationService reservationService, IMapper mapper, 
            IValidator<ReservationDTO> validator, IRoomService roomService, IFeaturedDealService featuredDealService)
        {
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Reservation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Reservation>>> GetAllReservationsAsync(int pageNumber = 0, int pageSize = 5)
        {
            const int maxPageSize = 10;

            if (pageNumber < 0)
            {
                return BadRequest("Page number should be 0 or greater.");
            }

            if (pageSize <= 0 || pageSize > maxPageSize)
            {
                return BadRequest($"Page size should be between 1 and {maxPageSize}.");
            }

            return Ok(await _reservationService.GetAllReservationsAsync(pageNumber, pageSize));
        }

        [HttpGet("{reservationId}" , Name = "GetReservationById")]
        [ProducesResponseType(typeof(Reservation), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Reservation>> GetReservationByIdAsync(int reservationId)
        {
            var reservationExists = await _reservationService.ReservationExists(reservationId);

            if (!reservationExists)
            {
                return NotFound($"Reservation with ID {reservationId} not found");
            }

            return Ok(await _reservationService.GetReservationByIdAsync(reservationId));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Reservation), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Reservation>> AddReservationAsync(ReservationDTO newReservation)
        {
            var validationResult = await _validator.ValidateAsync(newReservation);

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

            var room = await _roomService.GetRoomByIdAsync(newReservation.RoomId);

            if (!room.Available)
            {
                return BadRequest("Room Not Available!!");
            }

            var availableDate = await _reservationService.IsReservationAvailableAsync(newReservation.RoomId, newReservation.CheckIn, newReservation.CheckOut);

            if (!availableDate)
            {
                return BadRequest("Room is reserved in this period please change Dates!!");
            }

            var mappedReservation = _mapper.Map<Reservation>(newReservation);

            if (newReservation.IsFeaturedDeal)
            {
                var featuedDeal = _roomService.FeaturedDealByRoomId(newReservation.RoomId);
                mappedReservation.UpdatePrice(room.PricePerNight, featuedDeal.Discount);
            }
            else
            {
                mappedReservation.UpdatePrice(room.PricePerNight, 0);
            }

            mappedReservation.Id = await _reservationService.AddReservationAsync(mappedReservation);

            return CreatedAtRoute("GetReservationById",
                new
                {
                    reservationId = mappedReservation.Id
                },
                mappedReservation);
        }

        [HttpDelete("{reservationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteReservationAsync(int reservationId)
        {
            var reservationExists = await _reservationService.ReservationExists(reservationId);

            if (!reservationExists)
            {
                return NotFound($"FeaturedDeal with ID {reservationId} not found");
            }

            await _reservationService.DeleteReservationAsync(reservationId);

            return NoContent();
        }

        [HttpPut("{reservationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateReservationAsync(int reservationId, ReservationDTO updatedReservation)
        {
            var reservationExists = await _reservationService.ReservationExists(reservationId);

            if (!reservationExists)
            {
                return NotFound($"Reservation with ID {reservationId} not found");
            }

            var validationResult = await _validator.ValidateAsync(updatedReservation);

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

            var room = await _roomService.GetRoomByIdAsync(updatedReservation.RoomId);

            if (!room.Available)
            {
                return BadRequest("Room Not Available!!");
            }

            var availableDate = await _reservationService.IsReservationAvailableAsync(updatedReservation.RoomId, updatedReservation.CheckIn, updatedReservation.CheckOut);

            if (!availableDate)
            {
                return BadRequest("Room is reserved in this period please change Dates!!");
            }

            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);

            _mapper.Map(updatedReservation, reservation);

            await _reservationService.UpdateReservationAsync(reservation);

            return NoContent();
        }
    }
}
