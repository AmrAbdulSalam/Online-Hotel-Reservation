using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models.CityModel;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Api.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly IMapper _mapper;
        private readonly IValidator<CityDTO> _validator;

        public CityController(ICityService cityService , IMapper mapper , IValidator<CityDTO> validator)
        {
            _cityService = cityService ?? throw new ArgumentNullException(nameof(cityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        [HttpGet]
        public async Task<ActionResult<List<City>>> GetAllCitiesAsync()
        {
            return Ok(await _cityService.GetAllCitiesAsync());
        }

        [HttpGet("{cityId}" , Name ="GetCityById")]
        public async Task<ActionResult<City>> GetCityById(int cityId)
        {
            var cityExists = await _cityService.CityExists(cityId);

            if (!cityExists)
            {
                return NotFound($"City with ID {cityId} not found");
            }

            return await _cityService.GetCityByIdAsync(cityId);
        }

        [HttpPost]
        public async Task<ActionResult<City>> AddCityAsync(CityDTO newCity)
        {
            var validationResult = await _validator.ValidateAsync(newCity);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => new
                {
                    Property = error.PropertyName,
                    Error = error.ErrorMessage
                }).ToList();

                return BadRequest(new { Errors = errors });
            }

            var mappedCity = _mapper.Map<City>(newCity);

            mappedCity.Id = await _cityService.AddCityAsync(mappedCity);

            return CreatedAtRoute("GetCityById",
                new
                {
                    cityId = mappedCity.Id
                },
                mappedCity);
        }

        [HttpDelete("{cityId}")]
        public async Task<ActionResult> DeleteCityAsync(int cityId)
        {
            var cityExists = await _cityService.CityExists(cityId);

            if (!cityExists)
            {
                return NotFound($"City with ID {cityId} not found");
            }

            await _cityService.DeleteCityAsync(cityId);

            return NoContent();
        }

        [HttpPut("{cityId}")]
        public async Task<ActionResult> UpdateCityAsync(int cityId , CityDTO updatedCity)
        {
            var cityExists = await _cityService.CityExists(cityId);

            if (!cityExists)
            {
                return NotFound($"City with ID {cityId} not found");
            }

            var validationResult = await _validator.ValidateAsync(updatedCity);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => new
                {
                    Property = error.PropertyName,
                    Error = error.ErrorMessage
                }).ToList();

                return BadRequest(new { Errors = errors });
            }

            var city = _mapper.Map<City>(updatedCity);

            city.Id = cityId;

            city.UpdateModificationDate();

            await _cityService.UpdateCityAsync(city);

            return NoContent();
        }
    }
}
