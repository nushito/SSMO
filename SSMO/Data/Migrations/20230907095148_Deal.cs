using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class Deal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DealDescriptionBg",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealDescriptionEng",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealTypeBg",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealTypeEng",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DealDescriptionBg",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DealDescriptionEng",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DealTypeBg",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DealTypeEng",
                table: "Documents");
        }
    }
}
