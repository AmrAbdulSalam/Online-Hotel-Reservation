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
        private readonly ILogger<RoomController> _logger;
        private string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        public RoomController(IRoomService roomService, IMapper mapper, IValidator<RoomDTO> validator, IConfiguration configuration
            ,ILogger<RoomController> logger)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Get all rooms
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>       
        /// <remarks>
        /// Route Defualts:
        ///  
        ///     { 
        ///     Defualt:
        ///         PageNumber=0,
        ///         Count=5
        ///     
        ///     Max:
        ///         Count=10
        ///     }
        ///     
        /// Sample request-1:
        ///     
        ///     GET api/rooms
        ///     
        /// Sample request-2:
        /// 
        ///     GET api/rooms?pageNumber=0&pageSize=4
        ///     
        /// </remarks>
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


        /// <summary>
        /// Get a room by ID
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>        
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/rooms/10
        ///     
        /// </remarks>
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


        /// <summary>
        /// Get the featured deals for a room by an ID
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>     
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/rooms/1/featured-deal
        ///     
        /// </remarks>
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


        /// <summary>
        /// Create and add a new room
        /// </summary>
        /// <param name="newRoom"></param>
        /// <returns></returns>        
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST api/rooms
        ///     {
        ///         "Available": true,
        ///         "RoomNumber": "G0100",
        ///         "Type": "Luxury",
        ///         "Image": *UploadFile*,
        ///         "AdultCapacity": 3,
        ///         "ChildrenCapacity": 0,
        ///         "PricePerNight": 50.10,
        ///         "HotelId": 12
        ///     }
        ///     
        /// </remarks>
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
                _logger.LogError(ex, "Error while adding a room");
                return BadRequest();
            }
        }


        /// <summary>
        /// Delete a room by ID
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>       
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE api/rooms/10
        ///     
        /// </remarks>
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


        /// <summary>
        /// Update an existing room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="updatedRoom"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT api/rooms/10
        ///     {
        ///         "Available": true,
        ///         "RoomNumber": "G0100",
        ///         "Type": "Luxury",
        ///         "Image": *UploadFile*,
        ///         "AdultCapacity": 3,
        ///         "ChildrenCapacity": 0,
        ///         "PricePerNight": 50.10,
        ///         "HotelId": 12
        ///     }
        ///     
        /// </remarks>
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
                _logger.LogError(ex, "Error while adding a room");
                return BadRequest();
            }
        }
    }
}
