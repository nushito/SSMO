using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class DOcumentdEraseCustomerOrderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_Documents_DocumentId",
                table: "CustomerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_CustomerOrders_CustomerOrderId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CustomerOrderId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrders_DocumentId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "CustomerOrderId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "CustomerOrders");

            migrationBuilder.CreateTable(
                name: "CustomerOrderDocument",
                columns: table => new
                {
                    CustomerOrdersId = table.Column<int>(type: "int", nullable: false),
                    DocumentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrderDocument", x => new { x.CustomerOrdersId, x.DocumentsId });
                    table.ForeignKey(
                        name: "FK_CustomerOrderDocument_CustomerOrders_CustomerOrdersId",
                        column: x => x.CustomerOrdersId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrderDocument_Documents_DocumentsId",
                        column: x => x.DocumentsId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderDocument_DocumentsId",
                table: "CustomerOrderDocument",
                column: "DocumentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOrderDocument");

            migrationBuilder.AddColumn<int>(
                name: "CustomerOrderId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "CustomerOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CustomerOrderId",
                table: "Documents",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_DocumentId",
                table: "CustomerOrders",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrders_Documents_DocumentId",
                table: "CustomerOrders",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_CustomerOrders_CustomerOrderId",
                table: "Documents",
                column: "CustomerOrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
