using HotelReservation.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace HotelReservation.Test
{
    public class EncryptionServiceTest
    {
        private readonly EncryptionService _encryptionService;

        public EncryptionServiceTest()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<EncryptionService>>();

            _encryptionService = new EncryptionService(mockConfiguration.Object, mockLogger.Object);
        }

        [Fact]
        public void EncryptDecrypt_Successful()
        {
            var plainText = "Password123";

            var encryptedText = _encryptionService.Encrypt(plainText);

            Assert.NotEqual(plainText, encryptedText);
        }

        [Fact]
        public void EncryptDecrypt_NullInput_ReturnsNull()
        {
            string encryptedText = _encryptionService.Encrypt(null);
            string decryptedText = _encryptionService.Decrypt(null);

            Assert.Null(encryptedText);
            Assert.Null(decryptedText);
        }
    }
}
