using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class Precision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantity",
                table: "SupplierOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAvance",
                table: "SupplierOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "SupplierOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "SupplierOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "ServiceOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityM2",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderedQuantity",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "LoadedQuantityM3",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "PurchaseTransportCost",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProcentComission",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAvance",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "OtherExpenses",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Factoring",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Duty",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTrasnportCost",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "CustomsExpenses",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyExchangeRateUsdToBGN",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "BankExpenses",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Documents",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantity",
                table: "CustomerOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "CustomerOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotal",
                table: "CustomerOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAvance",
                table: "CustomerOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "CustomerOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "CustomerOrders",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantity",
                table: "SupplierOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAvance",
                table: "SupplierOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "SupplierOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "SupplierOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "ServiceOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityM2",
                table: "Products",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderedQuantity",
                table: "Products",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "LoadedQuantityM3",
                table: "Products",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Products",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "PurchaseTransportCost",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "ProcentComission",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAvance",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "OtherExpenses",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Factoring",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Duty",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryTrasnportCost",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "CustomsExpenses",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyExchangeRateUsdToBGN",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "BankExpenses",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Documents",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantity",
                table: "CustomerOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "CustomerOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotal",
                table: "CustomerOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAvance",
                table: "CustomerOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "CustomerOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "CustomerOrders",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)",
                oldPrecision: 18,
                oldScale: 5);
        }
    }
}
