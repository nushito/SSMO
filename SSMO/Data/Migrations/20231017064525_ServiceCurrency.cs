using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class ServiceCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "ServiceOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_CurrencyId",
                table: "ServiceOrders",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Currencies_CurrencyId",
                table: "ServiceOrders",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Currencies_CurrencyId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_CurrencyId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "ServiceOrders");
        }
    }
}
