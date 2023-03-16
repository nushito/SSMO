using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CreditNoteForProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditNoteId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreditNotePallets",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditNotePrice",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditNoteQuantity",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CreditNoteSheetsPerPallet",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreditNoteId",
                table: "Products",
                column: "CreditNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Documents_CreditNoteId",
                table: "Products",
                column: "CreditNoteId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Documents_CreditNoteId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CreditNoteId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreditNoteId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreditNotePallets",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreditNotePrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreditNoteQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreditNoteSheetsPerPallet",
                table: "Products");
        }
    }
}
