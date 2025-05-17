using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiketsApp.Migrations
{
    /// <inheritdoc />
    public partial class Order5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AllRoles_SallerId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AllRoles_UserId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Events_EventId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EventId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SallerId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EventId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SallerId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SallerId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EventId1",
                table: "Orders",
                column: "EventId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SallerId1",
                table: "Orders",
                column: "SallerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId1",
                table: "Orders",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AllRoles_SallerId1",
                table: "Orders",
                column: "SallerId1",
                principalTable: "AllRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AllRoles_UserId1",
                table: "Orders",
                column: "UserId1",
                principalTable: "AllRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Events_EventId1",
                table: "Orders",
                column: "EventId1",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
