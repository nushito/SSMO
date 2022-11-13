using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class MyCompanyToDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyCompanyId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_MyCompanyId",
                table: "Documents",
                column: "MyCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_MyCompanies_MyCompanyId",
                table: "Documents",
                column: "MyCompanyId",
                principalTable: "MyCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_MyCompanies_MyCompanyId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_MyCompanyId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MyCompanyId",
                table: "Documents");
        }
    }
}
