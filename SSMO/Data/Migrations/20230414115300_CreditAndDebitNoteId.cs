using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CreditAndDebitNoteId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditNoteId",
                table: "InvoiceProductDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DebitNoteId",
                table: "InvoiceProductDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_CreditNoteId",
                table: "InvoiceProductDetails",
                column: "CreditNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_DebitNoteId",
                table: "InvoiceProductDetails",
                column: "DebitNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProductDetails_Documents_CreditNoteId",
                table: "InvoiceProductDetails",
                column: "CreditNoteId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProductDetails_Documents_DebitNoteId",
                table: "InvoiceProductDetails",
                column: "DebitNoteId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProductDetails_Documents_CreditNoteId",
                table: "InvoiceProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProductDetails_Documents_DebitNoteId",
                table: "InvoiceProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceProductDetails_CreditNoteId",
                table: "InvoiceProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceProductDetails_DebitNoteId",
                table: "InvoiceProductDetails");

            migrationBuilder.DropColumn(
                name: "CreditNoteId",
                table: "InvoiceProductDetails");

            migrationBuilder.DropColumn(
                name: "DebitNoteId",
                table: "InvoiceProductDetails");
        }
    }
}
