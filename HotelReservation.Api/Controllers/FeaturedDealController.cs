using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using HotelReservation.Domain.ServiceInterfaces;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;

namespace HotelReservation.Api.Controllers
{
    [Route("api/featured-deals")]
    [ApiController]
    public class FeaturedDealController : ControllerBase
    {
        private readonly IFeaturedDealService _featuredDealService;
        private readonly IMapper _mapper;
        private readonly IValidator<FeaturedDealDTO> _validator;

        public FeaturedDealController(IFeaturedDealService featuredDealService, IMapper mapper , IValidator<FeaturedDealDTO> validator)
        {
            _featuredDealService = featuredDealService ?? throw new ArgumentNullException(nameof(featuredDealService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        [HttpGet]
        public async Task<ActionResult<List<FeaturedDealController>>> GetAllFeaturedDealsAsync()
        {
            return Ok(await _featuredDealService.GetAllFeaturedDealsAsync());
        }

        [HttpGet("{featuredDealId}" , Name = "GetFeaturedDealById")]
        public async Task<ActionResult<FeaturedDeal>> GetFeaturedDealByIdAsync(int featuredDealId)
        {
            var featuredDealExists = await _featuredDealService.FeaturedDealExists(featuredDealId);

            if (!featuredDealExists)
            {
                return NotFound($"FeaturedDeal with ID {featuredDealId} not found");
            }

            return Ok(await _featuredDealService.GetFeaturedDealByIdAsync(featuredDealId));
        }

        [HttpPost]
        public async Task<ActionResult<FeaturedDeal>> AddFeaturedDealAsync(FeaturedDealDTO newFeaturedDeal)
        {
            var validationResult = await _validator.ValidateAsync(newFeaturedDeal);

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

            var mappedFeaturedDeal = _mapper.Map<FeaturedDeal>(newFeaturedDeal);

            mappedFeaturedDeal.Id = await _featuredDealService.AddFeaturedDealAsync(mappedFeaturedDeal);

            return CreatedAtRoute("GetFeaturedDealById",
                new
                {
                    featuredDealId = mappedFeaturedDeal.Id
                },
                mappedFeaturedDeal);
        }

        [HttpDelete("{featuredDealId}")]
        public async Task<ActionResult> DeleteFeaturedDealAsync(int featuredDealId)
        {
            var featuredDealExists = await _featuredDealService.FeaturedDealExists(featuredDealId);

            if (!featuredDealExists)
            {
                return NotFound($"FeaturedDeal with ID {featuredDealId} not found");
            }

            await _featuredDealService.DeleteFeaturedDealAsync(featuredDealId);

            return NoContent();
        }

        [HttpPut("{featuredDealId}")]
        public async Task<ActionResult> UpdateFeaturedDealAsync(int featuredDealId , FeaturedDealDTO updatedFeaturedDeal)
        {
            var featuredDealExists = await _featuredDealService.FeaturedDealExists(featuredDealId);

            if (!featuredDealExists)
            {
                return NotFound($"FeaturedDeal with ID {featuredDealId} not found");
            }

            var validationResult = await _validator.ValidateAsync(updatedFeaturedDeal);

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

            var mappedFeaturedDeal = _mapper.Map<FeaturedDeal>(updatedFeaturedDeal);

            mappedFeaturedDeal.Id = featuredDealId;

            await _featuredDealService.UpdateFeaturedDealAsync(mappedFeaturedDeal);

            return NoContent();
        }
    }
}
