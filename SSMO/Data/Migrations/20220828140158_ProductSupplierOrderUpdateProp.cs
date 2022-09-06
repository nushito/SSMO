using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class ProductSupplierOrderUpdateProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSupplierOrder");

            migrationBuilder.AddColumn<int>(
                name: "SupplierOrderId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierOrderId",
                table: "Products",
                column: "SupplierOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SupplierOrders_SupplierOrderId",
                table: "Products",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SupplierOrders_SupplierOrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierOrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierOrderId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ProductSupplierOrder",
                columns: table => new
                {
                    ProductsId = table.Column<int>(type: "int", nullable: false),
                    SupplierOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSupplierOrder", x => new { x.ProductsId, x.SupplierOrdersId });
                    table.ForeignKey(
                        name: "FK_ProductSupplierOrder_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSupplierOrder_SupplierOrders_SupplierOrdersId",
                        column: x => x.SupplierOrdersId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSupplierOrder_SupplierOrdersId",
                table: "ProductSupplierOrder",
                column: "SupplierOrdersId");
        }
    }
}
