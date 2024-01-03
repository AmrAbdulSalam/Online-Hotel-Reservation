﻿using AutoMapper;
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
        [ProducesResponseType(typeof(List<FeaturedDealController>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<FeaturedDealController>>> GetAllFeaturedDealsAsync(int pageNumber = 0, int pageSize = 5)
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

            return Ok(await _featuredDealService.GetAllFeaturedDealsAsync(pageNumber, pageSize));
        }

        [HttpGet("{featuredDealId}" , Name = "GetFeaturedDealById")]
        [ProducesResponseType(typeof(FeaturedDeal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(typeof(FeaturedDeal), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
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
