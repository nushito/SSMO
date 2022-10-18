using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class InvoicesToCustomers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CustomerId",
                table: "Documents",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Customers_CustomerId",
                table: "Documents",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Customers_CustomerId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CustomerId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Documents");
        }
    }
}
