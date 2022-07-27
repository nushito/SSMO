using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class ProductUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOrderProduct");

            migrationBuilder.AddColumn<int>(
                name: "CustomerOrderId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerOrderId",
                table: "Products",
                column: "CustomerOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CustomerOrders_CustomerOrderId",
                table: "Products",
                column: "CustomerOrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CustomerOrders_CustomerOrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CustomerOrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomerOrderId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "CustomerOrderProduct",
                columns: table => new
                {
                    CustomerOrdersId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrderProduct", x => new { x.CustomerOrdersId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_CustomerOrderProduct_CustomerOrders_CustomerOrdersId",
                        column: x => x.CustomerOrdersId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrderProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderProduct_ProductsId",
                table: "CustomerOrderProduct",
                column: "ProductsId");
        }
    }
}
