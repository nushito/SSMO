using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class ImageDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FooterId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeaderId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_FooterId",
                table: "Documents",
                column: "FooterId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_HeaderId",
                table: "Documents",
                column: "HeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Images_FooterId",
                table: "Documents",
                column: "FooterId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Images_HeaderId",
                table: "Documents",
                column: "HeaderId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Images_FooterId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Images_HeaderId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_FooterId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_HeaderId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FooterId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "HeaderId",
                table: "Documents");
        }
    }
}
