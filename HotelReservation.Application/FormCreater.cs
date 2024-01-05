using HotelReservation.Domain;
using HotelReservation.Domain.Models;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelReservation.Application
{
    public class FormCreater : IFormCreater
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelRepository _hotelRepository;

        public FormCreater(IConfiguration configuration,IUserRepository userRepository,IRoomRepository roomRepository,
            IHotelRepository hotelRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
        }

        public async Task<string> CreateFormAsync(Reservation reservation)
        {
            var user = await _userRepository.GetUserByIdAsync(reservation.UserId);
            var room = await _roomRepository.GetRoomByIdAsync(reservation.RoomId);
            var hotel = await _hotelRepository.GetHotelByIdAsync(room.HotelId);

            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text("Online Hotel and Accommodation Reservation!!")
                        .SemiBold().FontSize(32).FontColor(Colors.Orange.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);

                            x.Item().Text("Welcome to our online hotel reservation platform!")
                                .FontSize(24);

                            x.Item().Text("We're thrilled to have you join us on this exciting journey of planning your stay.")
                                .FontSize(16);

                            x.Item().Text("This personalized reservation document contains all the details you need:")
                                .FontSize(16);

                            x.Item().Text("1- Your unique reservation reference number.")
                                .FontSize(16);

                            x.Item().Text("2- Check-in and check-out dates.")
                                .FontSize(16);

                            x.Item().Text("3- Pricing details and more.")
                                .FontSize(16);

                            x.Item().Text($"Reservation Details for {reservation.ReferenceceNo}")
                                .FontSize(24);

                            x.Item().Text($"Check-in: {reservation.CheckIn.ToString("dd/MM/yyyy")}")
                                .FontSize(16);

                            x.Item().Text($"Check-out: {reservation.CheckOut.ToString("dd/MM/yyyy")}")
                                .FontSize(16);

                            x.Item().Text($"Price: {reservation.Price}")
                                .FontSize(16);
                        });
                });

                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Item().Text("User Information")
                                .FontSize(26)
                                .FontColor(Colors.Green.Medium);

                            x.Item().Text($"Username: {user.Username}")
                                .FontSize(16);

                            x.Item().Text($"Email: {user.Email}")
                                .FontSize(16);

                            x.Item().Text("Hotel Details")
                                .FontSize(26)
                                .FontColor(Colors.Green.Medium);

                            x.Item().Text($"Hotel Name: {hotel.Name}")
                                .FontSize(16);

                            x.Item().Text($"Hotel Rate: {hotel.StarRate}")
                                .FontSize(16);

                            x.Item().Text($"Hotel Address: {hotel.Address}")
                                .FontSize(16);

                            x.Item().Text($"Hotel Description: {hotel.Description}")
                                .FontSize(16);

                            x.Item().Image($"{hotel.Image}");

                            x.Item().Text("Room Details")
                                .FontSize(26)
                                .FontColor(Colors.Green.Medium);

                            x.Item().Text($"Room Number: {room.RoomNumber}")
                                .FontSize(16);

                            x.Item().Text($"Room Type: {room.Type}")
                                .FontSize(16);

                            x.Item().Text($"Room AdultCapacity: {room.AdultCapacity}")
                                .FontSize(16);

                            x.Item().Text($"Room ChildrenCapacity: {room.ChildrenCapacity}")
                                .FontSize(16);

                            x.Item().Text($"Room PricePerNight: {room.PricePerNight}")
                                .FontSize(16);

                            x.Item().Text($"Room Type: {room.Type}")
                                .FontSize(16);

                            x.Item().Image($"{room.Image}");
                        });
                });
            })
            .GeneratePdf($"{_configuration["ImagePath:DirectoryPath"]}\\{reservation.ReferenceceNo}.pdf");

            return $"{_configuration["ImagePath:DirectoryPath"]}\\{reservation.ReferenceceNo}.pdf";
        }
    }
}
