using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiketsApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_BannedByAdmin",
                table: "AllRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "User_BannedByAdmin",
                table: "AllRoles",
                type: "bit",
                nullable: true);
        }
    }
}
