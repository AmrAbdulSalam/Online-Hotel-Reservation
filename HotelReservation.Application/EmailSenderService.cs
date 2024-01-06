using System.Net;
using System.Net.Mail;
using HotelReservation.Domain;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HotelReservation.Application
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration configuration, IUserRepository userRepository, ILogger<EmailSenderService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> SendConfirmationEmail(Reservation reservation)
        {
            var senderEmail = _configuration["EmailService:HostEmail"];
            var senderPassword = _configuration["EmailService:HostPassword"];
            var domain = _configuration["EmailService:HostDomain"];

            var user = await _userRepository.GetUserByIdAsync(reservation.UserId);

            var mail = new MailMessage(senderEmail, user.Email);
            mail.Subject = "Reservation Checkout Confirmation";

            mail.Body = $"Dear {user.Username},\n\n" +
                      $"I hope this message finds you well.\n\n" +
                      $"I am pleased to confirm your reservation with us. Your unique reservation code is '{reservation.ReferenceceNo}'.\n\n" +
                      $"Please find attached the PDF document containing detailed information about your reservation, including:\n" +
                      $"- Check-in Date: {reservation.CheckIn:d}\n" +
                      $"- Check-out Date: {reservation.CheckOut:d}\n" +
                      $"Should you have any questions or require further assistance, do not hesitate to contact our team.\n\n" +
                      $"We are looking forward to welcoming you to  and ensuring your stay is comfortable and enjoyable.\n\n" +
                      $"Best regards,\nOnline-Reservation\nHotel-Hr-Team";

            var attachment = new Attachment(reservation.ReservationInfoPath);
            mail.Attachments.Add(attachment);

            var smtpClinet = new SmtpClient(domain, 587);
            smtpClinet.EnableSsl = true;
            smtpClinet.UseDefaultCredentials = false;
            smtpClinet.Credentials = new NetworkCredential(senderEmail,senderPassword);

            try
            {
                smtpClinet.Send(mail);
                return true;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while sending mail");
                return false;
            }
        }
    }
}
