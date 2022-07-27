using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class ProductNullProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BankExpenses",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomsExpenses",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryTrasnportCost",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Duty",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Factoring",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FiscalAgentExpenses",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherExpenses",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProcentComission",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseTransportCost",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankExpenses",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomsExpenses",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeliveryTrasnportCost",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Duty",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Factoring",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FiscalAgentExpenses",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OtherExpenses",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProcentComission",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchaseTransportCost",
                table: "Products");
        }
    }
}
