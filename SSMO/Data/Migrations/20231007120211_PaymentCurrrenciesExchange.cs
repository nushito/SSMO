using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class PaymentCurrrenciesExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyForCalculationsId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NewAmountPerExchangeRate",
                table: "Payments",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CurrencyForCalculationsId",
                table: "Payments",
                column: "CurrencyForCalculationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Currencies_CurrencyForCalculationsId",
                table: "Payments",
                column: "CurrencyForCalculationsId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Currencies_CurrencyForCalculationsId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CurrencyForCalculationsId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CurrencyForCalculationsId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "NewAmountPerExchangeRate",
                table: "Payments");
        }
    }
}
