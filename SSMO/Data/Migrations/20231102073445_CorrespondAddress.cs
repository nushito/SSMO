using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class CorrespondAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrespondBgCity",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondBgCountry",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondBgStreet",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondCity",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondCountry",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrespondStreet",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrespondBgCity",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CorrespondBgCountry",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CorrespondBgStreet",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CorrespondCity",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CorrespondCountry",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CorrespondStreet",
                table: "Addresses");
        }
    }
}
