﻿using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Api.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly IValidator<PaymentDTO> _validator;

        public PaymentController(IPaymentService paymentService, IMapper mapper, IValidator<PaymentDTO> validator)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        [HttpGet]
        public async Task<ActionResult<List<Payment>>> GetAllPaymentsAsync()
        {
            return Ok(await _paymentService.GetAllPaymentsAsync());
        }

        [HttpGet("{paymentId}" , Name = "GetPaymentById")]
        public async Task<ActionResult<Payment>> GetPaymentByIdAsync(int paymentId)
        {
            var paymentExists = await _paymentService.PaymentExists(paymentId);

            if (!paymentExists)
            {
                return NotFound($"Payment with ID {paymentId} not found");
            }

            return Ok(await _paymentService.GetPaymentByIdAsync(paymentId));
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> fdfd(PaymentDTO newPayment)
        {
            var validationResult = await _validator.ValidateAsync(newPayment);

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

            var mappedPayment = _mapper.Map<Payment>(newPayment);

            mappedPayment.Id = await _paymentService.AddPaymentAsync(mappedPayment);

            return CreatedAtRoute("GetPaymentById",
                new
                {
                    paymentId = mappedPayment.Id
                },
                mappedPayment);
        }

        [HttpDelete("{paymentId}")]
        public async Task<ActionResult> DeletePaymentAsync(int paymentId)
        {
            var paymentExists = await _paymentService.PaymentExists(paymentId);

            if (!paymentExists)
            {
                return NotFound($"Payment with ID {paymentId} not found");
            }

            await _paymentService.DeletePaymentAsync(paymentId);

            return NoContent();
        }

        [HttpPut("{paymentId}")]
        public async Task<ActionResult> UpdatePaymentAsync(int paymentId , PaymentDTO updatedPayment)
        {
            var paymentExists = await _paymentService.PaymentExists(paymentId);

            if (!paymentExists)
            {
                return NotFound($"Payment with ID {paymentId} not found");
            }

            var validationResult = await _validator.ValidateAsync(updatedPayment);

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

            var mappedPayment = _mapper.Map<Payment>(updatedPayment);

            mappedPayment.Id = paymentId;

            await _paymentService.UpdatePaymentAsync(mappedPayment);

            return NoContent();
        }
    }
}