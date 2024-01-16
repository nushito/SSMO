using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CostPriceCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostPriceCurrencyId",
                table: "PurchaseProductDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseProductDetails_CostPriceCurrencyId",
                table: "PurchaseProductDetails",
                column: "CostPriceCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseProductDetails_Currencies_CostPriceCurrencyId",
                table: "PurchaseProductDetails",
                column: "CostPriceCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseProductDetails_Currencies_CostPriceCurrencyId",
                table: "PurchaseProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseProductDetails_CostPriceCurrencyId",
                table: "PurchaseProductDetails");

            migrationBuilder.DropColumn(
                name: "CostPriceCurrencyId",
                table: "PurchaseProductDetails");
        }
    }
}
