using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class BgPriceAndAmountProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BgAmount",
                table: "Products",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BgPrice",
                table: "Products",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BgAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BgPrice",
                table: "Products");
        }
    }
}
