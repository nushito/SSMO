using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CustomerOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_SupplierOrders_SupplierOrderId",
                table: "CustomerOrders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrders_SupplierOrderId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "SupplierOrderId",
                table: "CustomerOrders");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_CustomerOrderId",
                table: "SupplierOrders",
                column: "CustomerOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrders_CustomerOrders_CustomerOrderId",
                table: "SupplierOrders",
                column: "CustomerOrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_CustomerOrders_CustomerOrderId",
                table: "SupplierOrders");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOrders_CustomerOrderId",
                table: "SupplierOrders");

            migrationBuilder.AddColumn<int>(
                name: "SupplierOrderId",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_SupplierOrderId",
                table: "CustomerOrders",
                column: "SupplierOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrders_SupplierOrders_SupplierOrderId",
                table: "CustomerOrders",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
