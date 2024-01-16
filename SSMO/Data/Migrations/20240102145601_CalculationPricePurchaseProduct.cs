using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CalculationPricePurchaseProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CalculationCurrencyPrice",
                table: "PurchaseProductDetails",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationCurrencyPrice",
                table: "PurchaseProductDetails");
        }
    }
}
