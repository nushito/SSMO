using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CreditInvoiceDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreditToInvoiceDate",
                table: "Documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreditToInvoiceNumber",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditToInvoiceDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreditToInvoiceNumber",
                table: "Documents");
        }
    }
}
