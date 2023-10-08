using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class PaymentCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstCurrency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SecondCurrency",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CurrencyId",
                table: "Payments",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Currencies_CurrencyId",
                table: "Payments",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Currencies_CurrencyId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CurrencyId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Payments");

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
        }
    }
}
