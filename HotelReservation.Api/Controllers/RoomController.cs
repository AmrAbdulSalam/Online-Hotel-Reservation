using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
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

        [HttpGet]
        public async Task<ActionResult<List<Room>>> GetAllRoomsAsync()
        {
            return Ok(await _roomService.GetAllRoomsAsync());
        }

        [HttpGet("{roomId}" , Name = "GetRoomById")]
        public async Task<ActionResult<Room>> GetRoomByIdAsync(int roomId)
        {
            var roomExists = await _roomService.RoomExists(roomId);

            if (!roomExists)
            {
                return NotFound($"Room with ID {roomId} not found");
            }

            return Ok(await _roomService.GetRoomByIdAsync(roomId));
        }

        [HttpPost]
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

        [HttpDelete("{roomId}")]
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

        [HttpPut("{roomId}")]
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
