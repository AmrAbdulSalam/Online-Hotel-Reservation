﻿using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using HotelReservation.Domain.ServiceInterfaces;

namespace HotelReservation.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<int> AddPaymentAsync(Payment payment)
        {
            return await _paymentRepository.AddPaymentAsync(payment);
        }

        public async Task DeletePaymentAsync(int paymentId)
        {
            await _paymentRepository.DeletePaymentAsync(paymentId);
        }

        public async Task<List<Payment>> GetAllPaymentsAsync(int pageNumber, int pageSize)
        {
            return await _paymentRepository.GetAllPaymentsAsync(pageNumber, pageSize);
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _paymentRepository.GetPaymentByIdAsync(paymentId);
        }

        public async Task<bool> PaymentExists(int paymentId)
        {
            return await _paymentRepository.PaymentExists(paymentId);
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            await _paymentRepository.UpdatePaymentAsync(payment);
        }
    }
}
