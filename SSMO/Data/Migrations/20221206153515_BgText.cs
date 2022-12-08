using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class BgText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BgName",
                table: "MyCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgRepresentativePerson",
                table: "MyCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgCustomerName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgCustomerRepresentativePerson",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgCity",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgStreet",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bgcountry",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BgName",
                table: "MyCompanies");

            migrationBuilder.DropColumn(
                name: "BgRepresentativePerson",
                table: "MyCompanies");

            migrationBuilder.DropColumn(
                name: "BgCustomerName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BgCustomerRepresentativePerson",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BgCity",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "BgStreet",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Bgcountry",
                table: "Addresses");
        }
    }
}
