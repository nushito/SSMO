using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class PurchaseAndInvoiceProductRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseProductDetailsId",
                table: "InvoiceProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_PurchaseProductDetailsId",
                table: "InvoiceProductDetails",
                column: "PurchaseProductDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProductDetails_PurchaseProductDetails_PurchaseProductDetailsId",
                table: "InvoiceProductDetails",
                column: "PurchaseProductDetailsId",
                principalTable: "PurchaseProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProductDetails_PurchaseProductDetails_PurchaseProductDetailsId",
                table: "InvoiceProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceProductDetails_PurchaseProductDetailsId",
                table: "InvoiceProductDetails");

            migrationBuilder.DropColumn(
                name: "PurchaseProductDetailsId",
                table: "InvoiceProductDetails");
        }
    }
}
