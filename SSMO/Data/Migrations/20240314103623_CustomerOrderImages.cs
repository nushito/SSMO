using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CustomerOrderImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FooterId",
                table: "CustomerOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeaderId",
                table: "CustomerOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_FooterId",
                table: "CustomerOrders",
                column: "FooterId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_HeaderId",
                table: "CustomerOrders",
                column: "HeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrders_Images_FooterId",
                table: "CustomerOrders",
                column: "FooterId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrders_Images_HeaderId",
                table: "CustomerOrders",
                column: "HeaderId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_Images_FooterId",
                table: "CustomerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_Images_HeaderId",
                table: "CustomerOrders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrders_FooterId",
                table: "CustomerOrders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrders_HeaderId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "FooterId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "HeaderId",
                table: "CustomerOrders");
        }
    }
}
