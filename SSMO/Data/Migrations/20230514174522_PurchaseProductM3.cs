using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class PurchaseProductM3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "QuantityM3",
                table: "PurchaseProductDetails",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityM3",
                table: "PurchaseProductDetails");
        }
    }
}
