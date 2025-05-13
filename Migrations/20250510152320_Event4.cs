using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiketsApp.Migrations
{
    /// <inheritdoc />
    public partial class Event4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventImage_Images_imagesId",
                table: "EventImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventImage",
                table: "EventImage");

            migrationBuilder.DropIndex(
                name: "IX_EventImage_imagesId",
                table: "EventImage");

            migrationBuilder.RenameColumn(
                name: "imagesId",
                table: "EventImage",
                newName: "EmagesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventImage",
                table: "EventImage",
                columns: new[] { "EmagesId", "EventsId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventImage_EventsId",
                table: "EventImage",
                column: "EventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventImage_Images_EmagesId",
                table: "EventImage",
                column: "EmagesId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventImage_Images_EmagesId",
                table: "EventImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventImage",
                table: "EventImage");

            migrationBuilder.DropIndex(
                name: "IX_EventImage_EventsId",
                table: "EventImage");

            migrationBuilder.RenameColumn(
                name: "EmagesId",
                table: "EventImage",
                newName: "imagesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventImage",
                table: "EventImage",
                columns: new[] { "EventsId", "imagesId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventImage_imagesId",
                table: "EventImage",
                column: "imagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventImage_Images_imagesId",
                table: "EventImage",
                column: "imagesId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
