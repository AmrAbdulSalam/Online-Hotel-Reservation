using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using HotelReservation.Domain.ServiceInterfaces;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Api.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IMapper _mapper;
        private readonly IValidator<HotelDTO> _validator;
        private readonly IConfiguration _configuration;
        private string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        public HotelController(IHotelService hotelService, IMapper mapper, IValidator<HotelDTO> validator, IConfiguration configuration)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>
        /// Get all hotel
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="hotelName"></param>
        /// <returns></returns>
        /// /// <remarks>
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
        ///     GET api/hotels
        ///     
        /// Sample request-2:
        ///     
        ///     GET api/hotels?pageNumber=0&pageSize=4
        ///     
        /// Sample request-3:
        ///
        ///     GET api/hotels?hotelName=Rotana
        /// 
        /// </remarks>
        [Authorize(Policy = "RequireUserOrAdminRole")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Hotel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Hotel>>> GetAllHotelsAsync(int pageNumber = 0, int pageSize = 5, string? hotelName = "")
        {
            var hotels = await _hotelService.GetAllHotelsAsync(0, int.MaxValue);

            if (!string.IsNullOrWhiteSpace(hotelName))
            {
                hotels = hotels.Where(city => city.Name.Contains(hotelName, StringComparison.OrdinalIgnoreCase)).ToList();

                if (hotels.Count == 0) return NotFound("Hotel Not Found");
            }

            const int maxPageSize = 10;

            if (pageNumber < 0)
            {
                return BadRequest("Page number should be 0 or greater.");
            }

            if (pageSize <= 0 || pageSize > maxPageSize)
            {
                return BadRequest($"Page size should be between 1 and {maxPageSize}.");
            }

            var paggingHotels = hotels.Skip(pageNumber * pageSize).Take(pageSize).ToList();

            return Ok(paggingHotels);
        }


        /// <summary>
        /// Get a hotel by ID
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/hotels/10
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireUserOrAdminRole")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{hotelId}", Name = "GetHotelById")]
        [ProducesResponseType(typeof(Hotel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Hotel>> GetHotelById(int hotelId)
        {
            var hotelExists = await _hotelService.HotelExists(hotelId);

            if (!hotelExists)
            {
                return NotFound($"Hotel with ID {hotelId} not found");
            }

            return Ok(await _hotelService.GetHotelByIdAsync(hotelId));
        }


        /// <summary>
        /// Create and add a new hotel
        /// </summary>
        /// <param name="newHotel"></param>
        /// <returns></returns>       
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST api/hotels
        ///     {
        ///         "Name": "Royal Suites",
        ///         "StarRate": "3",
        ///         "Description": "facilities to improve the quality and joy of your stay.",
        ///         "Address": "Rafedia-Street"
        ///         "Image" : *UploadFile*
        ///         "CityId": 1
        ///     }
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ProducesResponseType(typeof(Hotel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Hotel>> AddHotelAsync([FromForm] HotelDTO newHotel)
        {
            var validationResult = await _validator.ValidateAsync(newHotel);

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
                var fileExtension = Path.GetExtension(newHotel.Image.FileName);
                
                var fileName = $"{newHotel.Name}_{timestamp}{fileExtension}";

                var directoryPath = _configuration.GetValue<string>("ImagePath:DirectoryPath");

                var path = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    newHotel.Image.CopyTo(stream);
                }

                var mappedHotel = _mapper.Map<Hotel>(newHotel);

                mappedHotel.Image = path;
                mappedHotel.Id = await _hotelService.AddHotelAsync(mappedHotel);
                
                return CreatedAtRoute("GetHotelById",
                    new
                    {
                        hotelId = mappedHotel.Id
                    },
                    mappedHotel);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Delete a hotel by ID
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>        
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE api/hotels/10
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{hotelId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHotelAsync(int hotelId)
        {
            var hotelExists = await _hotelService.HotelExists(hotelId);

            if (!hotelExists)
            {
                return NotFound($"Hotel with ID {hotelId} not found");
            }

            await _hotelService.DeleteHotelAsync(hotelId);

            return NoContent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="updatedHotel"></param>
        /// <returns></returns>        
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT api/hotels/10
        ///     {
        ///         "Name": "Royal Suites",
        ///         "StarRate": "3",
        ///         "Description": "facilities to improve the quality and joy of your stay.",
        ///         "Address": "Rafedia-Street"
        ///         "Image" : *UploadFile*
        ///         "CityId": 1
        ///     }
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{hotelId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateHotelAsync(int hotelId ,[FromForm] HotelDTO updatedHotel)
        {
            var hotelExists = await _hotelService.HotelExists(hotelId);

            if (!hotelExists)
            {
                return NotFound($"Hotel with ID {hotelId} not found");
            }

            var validationResult = await _validator.ValidateAsync(updatedHotel);

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

            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);

            _mapper.Map(updatedHotel, hotel);

            hotel.UpdateModificationDate();

            try
            {
                if (updatedHotel.Image != null)
                {
                    var fileExtension = Path.GetExtension(updatedHotel.Image.FileName);

                    var fileName = $"{updatedHotel.Name}_{timestamp}{fileExtension}";

                    var directoryPath = _configuration.GetValue<string>("ImagePath:DirectoryPath");

                    var path = Path.Combine(directoryPath, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        updatedHotel.Image.CopyTo(stream);
                    }

                    hotel.Image = path;
                }

                await _hotelService.UpdateHotelAsync(hotel);

                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
