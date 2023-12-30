using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.RepositoryInterfaces
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllPaymentsAsync();

        Task<Payment> GetPaymentByIdAsync(int paymentId);

        Task AddPaymentAsync(Payment payment);

        Task UpdatePaymentAsync(Payment payment);

        Task DeletePaymentAsync(int paymentId);

        Task<bool> PaymentExists(int paymentId);
    }
}
