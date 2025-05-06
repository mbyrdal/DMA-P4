using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystemWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ForceAddIsCollectedToReservation : Migration
    {
        /// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
    		migrationBuilder.AddColumn<bool>(
        	name: "IsCollected",
        	table: "Reservations",
        	type: "bit",
        	nullable: false,
        	defaultValue: false);

    		migrationBuilder.AddColumn<DateTime>(
        	name: "CollectedAt",
        	table: "Reservations",
        	type: "datetime2",
        	nullable: true);
	}


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
