using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class ListSupplierOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplierOrders_CustomerOrderId",
                table: "SupplierOrders");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_CustomerOrderId",
                table: "SupplierOrders",
                column: "CustomerOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplierOrders_CustomerOrderId",
                table: "SupplierOrders");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_CustomerOrderId",
                table: "SupplierOrders",
                column: "CustomerOrderId",
                unique: true);
        }
    }
}
