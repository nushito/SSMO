using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class Banks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Eta",
                table: "ServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Etd",
                table: "ServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BankDetailsDocument",
                columns: table => new
                {
                    BankDetailsId = table.Column<int>(type: "int", nullable: false),
                    DocumentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDetailsDocument", x => new { x.BankDetailsId, x.DocumentsId });
                    table.ForeignKey(
                        name: "FK_BankDetailsDocument_BankDetails_BankDetailsId",
                        column: x => x.BankDetailsId,
                        principalTable: "BankDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankDetailsDocument_Documents_DocumentsId",
                        column: x => x.DocumentsId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDetailsDocument_DocumentsId",
                table: "BankDetailsDocument",
                column: "DocumentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankDetailsDocument");

            migrationBuilder.DropColumn(
                name: "Eta",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "Etd",
                table: "ServiceOrders");
        }
    }
}
