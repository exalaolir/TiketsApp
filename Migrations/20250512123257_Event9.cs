using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiketsApp.Migrations
{
    /// <inheritdoc />
    public partial class Event9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SallerId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_SallerId",
                table: "Events",
                column: "SallerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AllRoles_SallerId",
                table: "Events",
                column: "SallerId",
                principalTable: "AllRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AllRoles_SallerId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_SallerId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SallerId",
                table: "Events");
        }
    }
}
