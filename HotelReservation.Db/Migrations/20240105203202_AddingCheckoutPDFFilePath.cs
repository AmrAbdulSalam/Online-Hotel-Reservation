using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddingCheckoutPDFFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReservationInfoPath",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationInfoPath",
                table: "Reservations");
        }
    }
}
