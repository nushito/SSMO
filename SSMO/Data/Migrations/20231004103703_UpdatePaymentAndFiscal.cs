using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class UpdatePaymentAndFiscal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurruncyRateExchange",
                table: "Payments",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstCurrency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondCurrency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Eta",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingLine",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurruncyRateExchange",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FirstCurrency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SecondCurrency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Eta",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ShippingLine",
                table: "Documents");
        }
    }
}
