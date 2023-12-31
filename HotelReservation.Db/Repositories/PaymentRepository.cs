using AutoMapper;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db.Repositories
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly HotelReservationDbContext _dbContext;
        private readonly IMapper _mapper;

        public PaymentRepository(HotelReservationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            var mappedPayment = _mapper.Map<Models.Payment>(payment);

            await _dbContext.Payments.AddAsync(mappedPayment);

            _dbContext.SaveChanges();
        }

        public async Task DeletePaymentAsync(int paymentId)
        {
            var payment = await GetPaymentByIdAsync(paymentId);

            var mappedPayment = _mapper.Map<Models.Payment>(payment);

            _dbContext.Payments.Remove(mappedPayment);

            _dbContext.SaveChanges();
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            var payments = await _dbContext.Payments.ToListAsync();

            return _mapper.Map<List<Payment>>(payments);
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _dbContext.Payments.FindAsync(paymentId);

            return _mapper.Map<Payment>(payment);
        }

        public async Task<bool> PaymentExists(int paymentId)
        {
            return await _dbContext.Payments.AnyAsync(x => x.Id == paymentId);
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            if (!await PaymentExists(payment.Id))
            {
                throw new Exception("Payment not found");
            }

            var mappedPayment = _mapper.Map<Models.Payment>(payment);

            _dbContext.Payments.Update(mappedPayment);

            _dbContext.SaveChanges();
        }
    }
}
