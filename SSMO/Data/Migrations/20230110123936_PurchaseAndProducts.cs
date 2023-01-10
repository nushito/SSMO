using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class PurchaseAndProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_PurchaseDocumentId",
                table: "Products",
                column: "PurchaseDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Documents_PurchaseDocumentId",
                table: "Products",
                column: "PurchaseDocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Documents_PurchaseDocumentId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_PurchaseDocumentId",
                table: "Products");
        }
    }
}
