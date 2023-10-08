using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class FiscalAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FiscalAgentId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FiscalAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BgName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BgDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalAgents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_FiscalAgentId",
                table: "Documents",
                column: "FiscalAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_FiscalAgents_FiscalAgentId",
                table: "Documents",
                column: "FiscalAgentId",
                principalTable: "FiscalAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_FiscalAgents_FiscalAgentId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "FiscalAgents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_FiscalAgentId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FiscalAgentId",
                table: "Documents");
        }
    }
}
