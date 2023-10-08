using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class FiscalCO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FiscalAgentId",
                table: "CustomerOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_FiscalAgentId",
                table: "CustomerOrders",
                column: "FiscalAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrders_FiscalAgents_FiscalAgentId",
                table: "CustomerOrders",
                column: "FiscalAgentId",
                principalTable: "FiscalAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_FiscalAgents_FiscalAgentId",
                table: "CustomerOrders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrders_FiscalAgentId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "FiscalAgentId",
                table: "CustomerOrders");
        }
    }
}
