using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CreditAndDebitTotalAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreditNoteTotalAmount",
                table: "Documents",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebitNoteTotalAmount",
                table: "Documents",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditNoteTotalAmount",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DebitNoteTotalAmount",
                table: "Documents");
        }
    }
}
