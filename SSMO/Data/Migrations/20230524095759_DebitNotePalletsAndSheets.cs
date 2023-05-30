using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class DebitNotePalletsAndSheets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeditNotePallets",
                table: "InvoiceProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeditNoteSheetsPerPallet",
                table: "InvoiceProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeditNotePallets",
                table: "InvoiceProductDetails");

            migrationBuilder.DropColumn(
                name: "DeditNoteSheetsPerPallet",
                table: "InvoiceProductDetails");
        }
    }
}
