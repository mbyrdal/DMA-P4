using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystemWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToStorageItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add RowVersion clumn manually via SQL
            migrationBuilder.Sql("ALTER TABLE WEXO_DEPOT ADD RowVersion rowversion NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE WEXO_DEPOT DROP COLUMN RowVersion");
        }
    }
}
