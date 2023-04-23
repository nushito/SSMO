using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class DocumentProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseProductDetails_Statuses_StatusId",
                table: "PurchaseProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseProductDetails_StatusId",
                table: "PurchaseProductDetails");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "PurchaseProductDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "PurchaseProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseProductDetails_StatusId",
                table: "PurchaseProductDetails",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseProductDetails_Statuses_StatusId",
                table: "PurchaseProductDetails",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
