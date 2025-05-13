using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiketsApp.Migrations
{
    /// <inheritdoc />
    public partial class Event10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AllRoles_SallerId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "SallerId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AllRoles_SallerId",
                table: "Events",
                column: "SallerId",
                principalTable: "AllRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AllRoles_SallerId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "SallerId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AllRoles_SallerId",
                table: "Events",
                column: "SallerId",
                principalTable: "AllRoles",
                principalColumn: "Id");
        }
    }
}
