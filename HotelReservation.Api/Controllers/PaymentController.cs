using AutoMapper;
using FluentValidation;
using HotelReservation.Api.Models;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
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


        /// <summary>
        /// Get all payments
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
        ///     GET api/payments
        ///     
        /// Sample request-2:
        /// 
        ///     GET api/payments?pageNumber=0&pageSize=4
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Payment>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<Payment>>> GetAllPaymentsAsync(int pageNumber = 0, int pageSize = 5)
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

            return Ok(await _paymentService.GetAllPaymentsAsync(pageNumber, pageSize));
        }


        /// <summary>
        /// Get a payment by ID
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>    
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET api/payments/10
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{paymentId}" , Name = "GetPaymentById")]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Payment>> GetPaymentByIdAsync(int paymentId)
        {
            var paymentExists = await _paymentService.PaymentExists(paymentId);

            if (!paymentExists)
            {
                return NotFound($"Payment with ID {paymentId} not found");
            }

            return Ok(await _paymentService.GetPaymentByIdAsync(paymentId));
        }


        /// <summary>
        /// Create and add a new payment
        /// </summary>
        /// <param name="newPayment"></param>
        /// <returns></returns>       
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST api/payments
        ///     {
        ///         "PaymentStatus": "Pending",
        ///         "PaymentMethod": "Visa",
        ///         "Amount": 50,
        ///         "ReservationId": 6
        ///     }
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireUserRole")]
        [HttpPost]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Payment>> AddPaymentAsync(PaymentDTO newPayment)
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


        /// <summary>
        /// Delete a payment by ID
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>       
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE api/payments/10
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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


        /// <summary>
        /// Update an existing payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="updatedPayment"></param>
        /// <returns></returns>
        ///         /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT api/payments
        ///     {
        ///         "PaymentStatus": "Pending",
        ///         "PaymentMethod": "Visa",
        ///         "Amount": 50,
        ///         "ReservationId": 6
        ///     }
        ///     
        /// </remarks>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
