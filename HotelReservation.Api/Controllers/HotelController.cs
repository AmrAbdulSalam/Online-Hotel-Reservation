using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using HotelReservation.Domain.ServiceInterfaces;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

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

        [HttpGet]
        public async Task<ActionResult<List<Hotel>>> GetAllCitiesAsync()
        {
            return Ok(await _hotelService.GetAllHotelsAsync());
        }

        [HttpGet("{hotelId}", Name = "GetHotelById")]
        public async Task<ActionResult<Hotel>> GetCityById(int hotelId)
        {
            var hotelExists = await _hotelService.HotelExists(hotelId);

            if (!hotelExists)
            {
                return NotFound($"Hotel with ID {hotelId} not found");
            }

            return Ok(await _hotelService.GetHotelByIdAsync(hotelId));
        }

        [HttpPost]
        public async Task<ActionResult> AddCityAsync([FromForm] HotelDTO newHotel)
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

        [HttpDelete("{hotelId}")]
        public async Task<ActionResult> DeleteHotelAsync(int hotelId)
        {
            var cityExists = await _hotelService.HotelExists(hotelId);

            if (!cityExists)
            {
                return NotFound($"Hotel with ID {hotelId} not found");
            }

            await _hotelService.DeleteHotelAsync(hotelId);

            return NoContent();
        }

        [HttpPut("{hotelId}")]
        public async Task<ActionResult> UpdateCityAsync(int hotelId ,[FromForm] HotelDTO updatedHotel)
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
