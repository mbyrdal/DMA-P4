using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystemWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReturnedToReservationItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedAt",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Reservations",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "EquipmentName",
                table: "ReservationItems",
                newName: "Equipment");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "ReservationItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "ReservationItems");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Reservations",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Equipment",
                table: "ReservationItems",
                newName: "EquipmentName");

            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAt",
                table: "Reservations",
                type: "datetime2",
                nullable: true);
        }
    }
}
