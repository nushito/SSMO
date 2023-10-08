using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class BankCustomerOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankDetailsCustomerOrder",
                columns: table => new
                {
                    BankDetailsId = table.Column<int>(type: "int", nullable: false),
                    CustomerOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDetailsCustomerOrder", x => new { x.BankDetailsId, x.CustomerOrdersId });
                    table.ForeignKey(
                        name: "FK_BankDetailsCustomerOrder_BankDetails_BankDetailsId",
                        column: x => x.BankDetailsId,
                        principalTable: "BankDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankDetailsCustomerOrder_CustomerOrders_CustomerOrdersId",
                        column: x => x.CustomerOrdersId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDetailsCustomerOrder_CustomerOrdersId",
                table: "BankDetailsCustomerOrder",
                column: "CustomerOrdersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankDetailsCustomerOrder");
        }
    }
}
