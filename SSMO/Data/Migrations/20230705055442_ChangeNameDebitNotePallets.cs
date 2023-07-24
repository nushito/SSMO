using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class ChangeNameDebitNotePallets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeditNoteSheetsPerPallet",
                table: "InvoiceProductDetails",
                newName: "DebitNoteSheetsPerPallet");

            migrationBuilder.RenameColumn(
                name: "DeditNotePallets",
                table: "InvoiceProductDetails",
                newName: "DebitNotePallets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DebitNoteSheetsPerPallet",
                table: "InvoiceProductDetails",
                newName: "DeditNoteSheetsPerPallet");

            migrationBuilder.RenameColumn(
                name: "DebitNotePallets",
                table: "InvoiceProductDetails",
                newName: "DeditNotePallets");
        }
    }
}
