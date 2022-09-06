using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class OrderNumberToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoNumber",
                table: "CustomerOrders",
                newName: "OrderConfirmationNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderConfirmationNumber",
                table: "CustomerOrders",
                newName: "CoNumber");
        }
    }
}
