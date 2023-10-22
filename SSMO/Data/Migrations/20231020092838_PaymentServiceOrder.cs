using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class PaymentServiceOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Documents_DocumentId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_DocumentId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "TransportCompany",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "TransportCompanyAddress",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "TransportCompanyVat",
                table: "ServiceOrders");

            migrationBuilder.AddColumn<int>(
                name: "DocumentId1",
                table: "ServiceOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MyCompanyId",
                table: "ServiceOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransportCompanyId",
                table: "ServiceOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceOrderId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransportCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eik = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportCompanies_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_DocumentId",
                table: "ServiceOrders",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_DocumentId1",
                table: "ServiceOrders",
                column: "DocumentId1",
                unique: true,
                filter: "[DocumentId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_MyCompanyId",
                table: "ServiceOrders",
                column: "MyCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_TransportCompanyId",
                table: "ServiceOrders",
                column: "TransportCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ServiceOrderId",
                table: "Payments",
                column: "ServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportCompanies_AddressId",
                table: "TransportCompanies",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_ServiceOrders_ServiceOrderId",
                table: "Payments",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Documents_DocumentId",
                table: "ServiceOrders",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Documents_DocumentId1",
                table: "ServiceOrders",
                column: "DocumentId1",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_MyCompanies_MyCompanyId",
                table: "ServiceOrders",
                column: "MyCompanyId",
                principalTable: "MyCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_TransportCompanies_TransportCompanyId",
                table: "ServiceOrders",
                column: "TransportCompanyId",
                principalTable: "TransportCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_ServiceOrders_ServiceOrderId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Documents_DocumentId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Documents_DocumentId1",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_MyCompanies_MyCompanyId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_TransportCompanies_TransportCompanyId",
                table: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "TransportCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_DocumentId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_DocumentId1",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_MyCompanyId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_TransportCompanyId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ServiceOrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DocumentId1",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "MyCompanyId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "TransportCompanyId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "ServiceOrderId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "TransportCompany",
                table: "ServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransportCompanyAddress",
                table: "ServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransportCompanyVat",
                table: "ServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_DocumentId",
                table: "ServiceOrders",
                column: "DocumentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Documents_DocumentId",
                table: "ServiceOrders",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
