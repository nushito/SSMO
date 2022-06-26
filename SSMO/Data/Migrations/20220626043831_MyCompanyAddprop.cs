using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class MyCompanyAddprop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyCompanyId",
                table: "SupplierOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FSCClaim",
                table: "MyCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FSCSertificate",
                table: "MyCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FSCClaim",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FSCSertificate",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_MyCompanyId",
                table: "SupplierOrders",
                column: "MyCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrders_MyCompanies_MyCompanyId",
                table: "SupplierOrders",
                column: "MyCompanyId",
                principalTable: "MyCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_MyCompanies_MyCompanyId",
                table: "SupplierOrders");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOrders_MyCompanyId",
                table: "SupplierOrders");

            migrationBuilder.DropColumn(
                name: "MyCompanyId",
                table: "SupplierOrders");

            migrationBuilder.DropColumn(
                name: "FSCClaim",
                table: "MyCompanies");

            migrationBuilder.DropColumn(
                name: "FSCSertificate",
                table: "MyCompanies");

            migrationBuilder.DropColumn(
                name: "FSCClaim",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "FSCSertificate",
                table: "CustomerOrders");
        }
    }
}
