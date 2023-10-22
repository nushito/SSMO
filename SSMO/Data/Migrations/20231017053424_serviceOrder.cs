using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class serviceOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_CustomerOrders_CustomerOrderId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_SupplierOrders_SupplierOrderId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_CustomerOrderId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_SupplierOrderId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "CustomerOrderId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "SupplierOrderId",
                table: "ServiceOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerOrderId",
                table: "ServiceOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierOrderId",
                table: "ServiceOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_CustomerOrderId",
                table: "ServiceOrders",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_SupplierOrderId",
                table: "ServiceOrders",
                column: "SupplierOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_CustomerOrders_CustomerOrderId",
                table: "ServiceOrders",
                column: "CustomerOrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_SupplierOrders_SupplierOrderId",
                table: "ServiceOrders",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
