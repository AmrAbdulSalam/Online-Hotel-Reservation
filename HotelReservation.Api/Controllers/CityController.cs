using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models.CityModel;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
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


        /// <summary>
        /// Get all cities
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cityName"></param>
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
        ///     GET api/cities
        ///     
        /// Sample request-2:
        /// 
        ///     GET api/cities?pageNumber=0&pageSize=4
        ///     
        ///Sample request-3:
        ///
        ///     GET api/cities?cityName=Nablus
        /// 
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        [ProducesResponseType(typeof(List<City>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<City>>> GetAllCitiesAsync(int pageNumber = 0, int pageSize = 5, string? cityName = "")
        {
            var citites = await _cityService.GetAllCitiesAsync(0, int.MaxValue);

            if (!string.IsNullOrWhiteSpace(cityName))
            {
                citites = citites.Where(city => city.Name.Contains(cityName, StringComparison.OrdinalIgnoreCase)).ToList();

                if (citites.Count == 0) return NotFound("City Not Found");
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

            var paggingCities = citites.Skip(pageNumber * pageSize).Take(pageSize).ToList();

            return Ok(paggingCities);
        }


        /// <summary>
        /// Trending destinations / Most visited cities
        /// </summary>
        /// <returns>Top 5 most visited cities</returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/cities/most-visited-cities
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireUserOrAdminRole")]
        [HttpGet("most-visited-cities")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(List<City>), StatusCodes.Status200OK)]
        public ActionResult<List<City>> MostVistedCities()
        {
            return Ok(_cityService.MostVistedCities());
        }


        /// <summary>
        /// Get a city by ID
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/cities/10
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{cityId}" , Name ="GetCityById")]
        [ProducesResponseType(typeof(City), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<City>> GetCityById(int cityId)
        {
            var cityExists = await _cityService.CityExists(cityId);

            if (!cityExists)
            {
                return NotFound($"City with ID {cityId} not found");
            }

            return Ok(await _cityService.GetCityByIdAsync(cityId));
        }


        /// <summary>
        /// Create and add a new city
        /// </summary>
        /// <param name="newCity"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST api/cities
        ///     {
        ///         "Name": "Nablus",
        ///         "Country": "Palestine",
        ///         "PostOffice": "Nablus-post",
        ///         "Currency": "LIS"
        ///     }
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ProducesResponseType(typeof(City), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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


        /// <summary>
        /// Delete a city by ID
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>       
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE api/cities/10
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{cityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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


        /// <summary>
        /// Update an existing city
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="updatedCity"></param>
        /// <returns></returns>      
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT api/cities/10
        ///     {
        ///         "Name": "Nablus",
        ///         "Country": "Palestine",
        ///         "PostOffice": "Nablus-post",
        ///         "Currency": "LIS"
        ///      }
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{cityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
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

            var city = await _cityService.GetCityByIdAsync(cityId);

            _mapper.Map(updatedCity, city);

            city.UpdateModificationDate();

            await _cityService.UpdateCityAsync(city);

            return NoContent();
        }
    }
}
