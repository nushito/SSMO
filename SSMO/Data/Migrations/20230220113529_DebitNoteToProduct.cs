using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class DebitNoteToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DebitNoteAmount",
                table: "Products",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DebitNoteId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DebitNotePrice",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebitNoteQuantity",
                table: "Products",
                type: "decimal(18,5)",
                precision: 18,
                scale: 5,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DebitToInvoiceDate",
                table: "Documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DebitToInvoiceNumber",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_DebitNoteId",
                table: "Products",
                column: "DebitNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Documents_DebitNoteId",
                table: "Products",
                column: "DebitNoteId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Documents_DebitNoteId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DebitNoteId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DebitNoteAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DebitNoteId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DebitNotePrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DebitNoteQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DebitToInvoiceDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DebitToInvoiceNumber",
                table: "Documents");
        }
    }
}
