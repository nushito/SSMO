using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CreditProductAmountProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreditNoteProductAmount",
                table: "Products",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditNoteProductAmount",
                table: "Products");
        }
    }
}
