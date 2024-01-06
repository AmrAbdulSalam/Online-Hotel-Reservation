using System.Security.Cryptography;
using System.Text;
using HotelReservation.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HotelReservation.Application
{
    public class EncryptionService : IEncryptionService
    {
        private readonly string Key;
        private readonly string IV;
        private readonly ILogger<EncryptionService> _logger;

        public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
        {
            Key = configuration["EncryptionSettings:Key"];
            IV = configuration["EncryptionSettings:IV"];
            _logger = logger;
        }

        public string Encrypt(string plainText)
        {
            try
            {
                byte[] encryptedBytes;
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                    aesAlg.IV = Encoding.UTF8.GetBytes(IV);

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            encryptedBytes = msEncrypt.ToArray();
                        }
                    }
                }
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Encrypting password");
                return null;
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                string plaintext = null;
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                    aesAlg.IV = Encoding.UTF8.GetBytes(IV);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plaintext;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while Decrypting");
                return null;
            }
        }
    }
}
