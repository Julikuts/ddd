using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeliveryApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    courier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location_x = table.Column<int>(type: "integer", nullable: true),
                    location_y = table.Column<int>(type: "integer", nullable: true),
                    weight = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    occuredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transports",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    speed = table.Column<int>(type: "integer", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "couriers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    transport_id = table.Column<int>(type: "integer", nullable: false),
                    location_x = table.Column<int>(type: "integer", nullable: true),
                    location_y = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_couriers", x => x.id);
                    table.ForeignKey(
                        name: "FK_couriers_transports_transport_id",
                        column: x => x.transport_id,
                        principalTable: "transports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "transports",
                columns: new[] { "id", "name", "speed" },
                values: new object[,]
                {
                    { 1, "pedestrian", 1 },
                    { 2, "bicycle", 2 },
                    { 3, "scooter", 3 },
                    { 4, "car", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_couriers_transport_id",
                table: "couriers",
                column: "transport_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "couriers");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "outbox");

            migrationBuilder.DropTable(
                name: "transports");
        }
    }
}
