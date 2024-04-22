using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "location_x",
                table: "couriers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location_y",
                table: "couriers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_couriers_transports_transport_id",
                table: "couriers",
                column: "transport_id",
                principalTable: "transports",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_couriers_transports_transport_id",
                table: "couriers");

            migrationBuilder.DropColumn(
                name: "location_x",
                table: "couriers");

            migrationBuilder.DropColumn(
                name: "location_y",
                table: "couriers");
        }
    }
}
