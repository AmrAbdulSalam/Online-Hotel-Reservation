using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Api.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly IValidator<RoomDTO> _validator;
        private readonly IConfiguration _configuration;
        private string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        public RoomController(IRoomService roomService, IMapper mapper, IValidator<RoomDTO> validator, IConfiguration configuration)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [Authorize(Policy = "RequireUserOrAdminRole")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Room>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Room>>> GetAllRoomsAsync(int pageNumber = 0, int pageSize = 5)
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

            return Ok(await _roomService.GetAllRoomsAsync(pageNumber, pageSize));
        }

        [Authorize(Policy = "RequireUserOrAdminRole")]
        [HttpGet("{roomId}" , Name = "GetRoomById")]
        [ProducesResponseType(typeof(Room), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Room>> GetRoomByIdAsync(int roomId)
        {
            var roomExists = await _roomService.RoomExists(roomId);

            if (!roomExists)
            {
                return NotFound($"Room with ID {roomId} not found");
            }

            return Ok(await _roomService.GetRoomByIdAsync(roomId));
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{roomId}/featured-deal")]
        [ProducesResponseType(typeof(FeaturedDeal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeaturedDeal>> FeaturedDealByRoomId(int roomId)
        {
            var roomExists = await _roomService.RoomExists(roomId);

            if (!roomExists)
            {
                return NotFound($"Room with ID {roomId} not found");
            }

            return Ok(_roomService.FeaturedDealByRoomId(roomId));
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ProducesResponseType(typeof(Room), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Room>> AddRoomAsync([FromForm] RoomDTO newRoom)
        {
            var validationResult = await _validator.ValidateAsync(newRoom);

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

            try
            {
                var fileExtension = Path.GetExtension(newRoom.Image.FileName);

                var fileName = $"{newRoom.RoomNumber}_{timestamp}{fileExtension}";

                var directoryPath = _configuration.GetValue<string>("ImagePath:DirectoryPath");

                var path = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    newRoom.Image.CopyTo(stream);
                }

                var mappedRoom = _mapper.Map<Room>(newRoom);

                mappedRoom.Image = path;
                mappedRoom.Id = await _roomService.AddRoomAsync(mappedRoom);

                return CreatedAtRoute("GetRoomById",
                    new
                    {
                        roomId = mappedRoom.Id
                    },
                    mappedRoom);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{roomId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteRoomAsync(int roomId)
        {
            var roomExists = await _roomService.RoomExists(roomId);

            if (!roomExists)
            {
                return NotFound($"Room with ID {roomId} not found");
            }

            await _roomService.DeleteRoomAsync(roomId);

            return NoContent();
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{roomId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateRoomAsync(int roomId, [FromForm] RoomDTO updatedRoom)
        {
            var roomExists = await _roomService.RoomExists(roomId);

            if (!roomExists)
            {
                return NotFound($"Room with ID {roomId} not found");
            }

            var validationResult = await _validator.ValidateAsync(updatedRoom);

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

            var room = await _roomService.GetRoomByIdAsync(roomId);

            _mapper.Map(updatedRoom, room);

            room.UpdateModificationDate();

            try
            {
                if (updatedRoom.Image != null)
                {
                    var fileExtension = Path.GetExtension(updatedRoom.Image.FileName);

                    var fileName = $"{updatedRoom.RoomNumber}_{timestamp}{fileExtension}";

                    var directoryPath = _configuration.GetValue<string>("ImagePath:DirectoryPath");

                    var path = Path.Combine(directoryPath, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        updatedRoom.Image.CopyTo(stream);
                    }

                    room.Image = path;
                }

                await _roomService.UpdateRoomAsync(room);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
