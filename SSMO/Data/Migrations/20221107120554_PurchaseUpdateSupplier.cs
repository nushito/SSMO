using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class PurchaseUpdateSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SupplierId",
                table: "Documents",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Suppliers_SupplierId",
                table: "Documents",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Suppliers_SupplierId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_SupplierId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Documents");
        }
    }
}
