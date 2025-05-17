using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiketsApp.Migrations
{
    /// <inheritdoc />
    public partial class Order4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SallerId = table.Column<int>(type: "int", nullable: false),
                    Row = table.Column<int>(type: "int", nullable: true),
                    Seat = table.Column<int>(type: "int", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EventId1 = table.Column<int>(type: "int", nullable: true),
                    SallerId1 = table.Column<int>(type: "int", nullable: true),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AllRoles_SallerId",
                        column: x => x.SallerId,
                        principalTable: "AllRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_AllRoles_SallerId1",
                        column: x => x.SallerId1,
                        principalTable: "AllRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_AllRoles_UserId",
                        column: x => x.UserId,
                        principalTable: "AllRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_AllRoles_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AllRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Events_EventId1",
                        column: x => x.EventId1,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EventId",
                table: "Orders",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EventId1",
                table: "Orders",
                column: "EventId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SallerId",
                table: "Orders",
                column: "SallerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SallerId1",
                table: "Orders",
                column: "SallerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId1",
                table: "Orders",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
