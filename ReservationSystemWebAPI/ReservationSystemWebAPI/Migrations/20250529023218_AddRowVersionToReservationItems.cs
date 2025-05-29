using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystemWebAPI.Migrations
{
    public partial class AddRowVersionToReservationItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add rowversion column to ReservationItems
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ReservationItems",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ReservationItems");
        }
    }
}
