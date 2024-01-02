using HotelReservation.Domain.Models;

namespace HotelReservation.Domain.ServiceInterfaces
{
    public interface IPaymentService
    {
        Task<List<Payment>> GetAllPaymentsAsync();

        Task<Payment> GetPaymentByIdAsync(int paymentId);

        Task<int> AddPaymentAsync(Payment payment);

        Task UpdatePaymentAsync(Payment payment);

        Task DeletePaymentAsync(int paymentId);

        Task<bool> PaymentExists(int paymentId);
    }
}
