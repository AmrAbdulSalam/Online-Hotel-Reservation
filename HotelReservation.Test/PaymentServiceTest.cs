using HotelReservation.Application.Services;
using HotelReservation.Domain.Enums;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Moq;

namespace HotelReservation.Test
{
    public class PaymentServiceTest
    {
        private readonly Payment _payment;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;

        public PaymentServiceTest()
        {
            _payment = new Payment()
            {
                Id = 1,
                PaymentStatus = PaymentStatusEnum.Pending,
                PaymentMethod = PaymentMethodEnum.Visa,
                Amount = 100,
                ReservationId = 1
            };

            _paymentRepositoryMock = new Mock<IPaymentRepository>();
        }

        [Fact]
        public async Task AddPaymentAsync_ValidPayment_ReturnsPaymentId()
        {
            _paymentRepositoryMock.Setup(repo => repo.AddPaymentAsync(_payment)).ReturnsAsync(_payment.Id);
            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            var result = await paymentService.AddPaymentAsync(_payment);

            Assert.Equal(_payment.Id, result);
        }

        [Fact]
        public async Task GetPaymentByIdAsync_ExistingPaymentId_ReturnsPayment()
        {
            _paymentRepositoryMock.Setup(repo => repo.GetPaymentByIdAsync(_payment.Id)).ReturnsAsync(_payment);
            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            var result = await paymentService.GetPaymentByIdAsync(_payment.Id);

            Assert.Equal(_payment, result);
        }

        [Fact]
        public async Task GetPaymentByIdAsync_NonExistingPaymentId_ReturnsNull()
        {
            int nonExistingPaymentId = 99999;
            _paymentRepositoryMock.Setup(repo => repo.GetPaymentByIdAsync(nonExistingPaymentId)).ReturnsAsync((Payment)null);
            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            var result = await paymentService.GetPaymentByIdAsync(nonExistingPaymentId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllPaymentsAsync_ReturnsListOfPayments()
        {
            var payments = new List<Payment> { _payment };
            _paymentRepositoryMock.Setup(repo => repo.GetAllPaymentsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(payments);
            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            var result = await paymentService.GetAllPaymentsAsync(1, 10);

            Assert.Equal(payments, result);
        }

        [Fact]
        public async Task DeletePaymentAsync_ExistingPaymentId_DeletesPayment()
        {
            var existingPaymentId = _payment.Id;
            _paymentRepositoryMock.Setup(repo => repo.DeletePaymentAsync(existingPaymentId)).Returns(Task.CompletedTask);
            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            await paymentService.DeletePaymentAsync(existingPaymentId);

            _paymentRepositoryMock.Verify(repo => repo.DeletePaymentAsync(existingPaymentId), Times.Once);
        }

        [Fact]
        public async Task DeletePaymentAsync_NonExistingPaymentId_ThrowsException()
        {
            int nonExistingPaymentId = 99999;
            _paymentRepositoryMock.Setup(repo => repo.DeletePaymentAsync(nonExistingPaymentId)).ThrowsAsync(new ArgumentNullException());

            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await paymentService.DeletePaymentAsync(nonExistingPaymentId));
        }

        [Fact]
        public async Task PaymentExists_ExistingPaymentId_ReturnsTrue()
        {
            _paymentRepositoryMock.Setup(repo => repo.PaymentExists(_payment.Id)).ReturnsAsync(true);
            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            var result = await paymentService.PaymentExists(_payment.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task PaymentExists_NonExistingPaymentId_ReturnsFalse()
        {
            int nonExistingPaymentId = 99999;
            _paymentRepositoryMock.Setup(repo => repo.PaymentExists(nonExistingPaymentId)).ReturnsAsync(false);
            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            var result = await paymentService.PaymentExists(nonExistingPaymentId);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdatePaymentAsync_ExistingPayment_ReturnsUpdatedPayment()
        {
            var updatedPayment = new Payment
            {
                Id = 1,
                PaymentStatus = PaymentStatusEnum.Completed,
                PaymentMethod = PaymentMethodEnum.Cash,
                Amount = 100,
                ReservationId = 1
            };

            _paymentRepositoryMock.Setup(repo => repo.UpdatePaymentAsync(updatedPayment)).Returns(Task.CompletedTask);
            _paymentRepositoryMock.Setup(repo => repo.GetPaymentByIdAsync(_payment.Id)).ReturnsAsync(updatedPayment);

            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            await paymentService.UpdatePaymentAsync(updatedPayment);
            var result = await paymentService.GetPaymentByIdAsync(_payment.Id);

            Assert.Equal(updatedPayment.PaymentStatus, result.PaymentStatus);
            Assert.Equal(updatedPayment.PaymentMethod, result.PaymentMethod);
            Assert.Equal(updatedPayment.Amount, result.Amount);
            Assert.Equal(updatedPayment.ReservationId, result.ReservationId);
        }

        [Fact]
        public async Task UpdatePaymentAsync_NonExistingPayment_ThrowsException()
        {
            var nonExistingPaymentId = 99999;
            var updatedPayment = new Payment
            {
                Id = nonExistingPaymentId,
                PaymentStatus = PaymentStatusEnum.Completed,
                PaymentMethod = PaymentMethodEnum.Cash,
                Amount = 100,
                ReservationId = 1
            };

            _paymentRepositoryMock.Setup(repo => repo.UpdatePaymentAsync(updatedPayment)).ThrowsAsync(new Exception());

            var paymentService = new PaymentService(_paymentRepositoryMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await paymentService.UpdatePaymentAsync(updatedPayment));
        }
    }
}
