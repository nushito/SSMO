using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSMO.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BgStreet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BgCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bgcountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Descriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BgName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BgCustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EIK = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VAT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BgCustomerRepresentativePerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentativePerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MyCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BgName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Eik = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    VAT = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    BgRepresentativePerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentativePerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSCClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSCSertificate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyCompanies_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderConfirmationNumber = table.Column<int>(type: "int", nullable: false),
                    CustomerPoNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoadingPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryTerms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    MyCompanyId = table.Column<int>(type: "int", nullable: false),
                    FSCClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSCSertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    TotalSheets = table.Column<int>(type: "int", nullable: false),
                    TotalPallets = table.Column<int>(type: "int", nullable: false),
                    Vat = table.Column<int>(type: "int", nullable: true),
                    PaidAvance = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    PaidAmountStatus = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOrders_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerOrders_MyCompanies_MyCompanyId",
                        column: x => x.MyCompanyId,
                        principalTable: "MyCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerOrders_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Eik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VAT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankDetailId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    FSCClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSCSertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentativePerson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Swift = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDetails_MyCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "MyCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankDetails_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    CustomerOrderId = table.Column<int>(type: "int", nullable: false),
                    MyCompanyId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    PaidAvance = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    DatePaidAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidStatus = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FscClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FscSertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VAT = table.Column<int>(type: "int", nullable: true),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NetWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    LoadingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierOrders_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOrders_MyCompanies_MyCompanyId",
                        column: x => x.MyCompanyId,
                        principalTable: "MyCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOrders_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrderSupplierOrder",
                columns: table => new
                {
                    CustomerOrdersId = table.Column<int>(type: "int", nullable: false),
                    SupplierOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrderSupplierOrder", x => new { x.CustomerOrdersId, x.SupplierOrdersId });
                    table.ForeignKey(
                        name: "FK_CustomerOrderSupplierOrder_CustomerOrders_CustomerOrdersId",
                        column: x => x.CustomerOrdersId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrderSupplierOrder_SupplierOrders_SupplierOrdersId",
                        column: x => x.SupplierOrdersId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentNumber = table.Column<int>(type: "int", nullable: false),
                    PurchaseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditToInvoiceId = table.Column<int>(type: "int", nullable: false),
                    CreditToInvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DebitToInvoiceId = table.Column<int>(type: "int", nullable: false),
                    DebitToInvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FSCClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FSCSertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    SupplierOrderId = table.Column<int>(type: "int", nullable: false),
                    MyCompanyId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Incoterms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditNoteDeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetWeight = table.Column<decimal>(type: "decimal", nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal", nullable: false),
                    TruckNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Swb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseTransportCost = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    DeliveryTrasnportCost = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    BankExpenses = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Duty = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    CustomsExpenses = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Factoring = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    FiscalAgentExpenses = table.Column<decimal>(type: "decimal", nullable: false),
                    ProcentComission = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    OtherExpenses = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    PaidAvance = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    DatePaidAmount = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidStatus = table.Column<bool>(type: "bit", nullable: false),
                    CurrencyExchangeRateUsdToBGN = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Vat = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditNoteTotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DebitNoteTotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealTypeEng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealDescriptionEng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealTypeBg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealDescriptionBg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseProductDetailsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_MyCompanies_MyCompanyId",
                        column: x => x.MyCompanyId,
                        principalTable: "MyCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrderDocument",
                columns: table => new
                {
                    CustomerOrdersId = table.Column<int>(type: "int", nullable: false),
                    DocumentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrderDocument", x => new { x.CustomerOrdersId, x.DocumentsId });
                    table.ForeignKey(
                        name: "FK_CustomerOrderDocument_CustomerOrders_CustomerOrdersId",
                        column: x => x.CustomerOrdersId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrderDocument_Documents_DocumentsId",
                        column: x => x.DocumentsId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SupplierOrderId = table.Column<int>(type: "int", nullable: true),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    CustomerOrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_CustomerOrders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescriptionId = table.Column<int>(type: "int", nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    QuantityM3 = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: true),
                    LoadedQuantityM3 = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    QuantityLeftForPurchaseLoading = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    QuantityAvailableForCustomerOrder = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    SoldQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pallets = table.Column<int>(type: "int", nullable: false),
                    SheetsPerPallet = table.Column<int>(type: "int", nullable: false),
                    TotalSheets = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    BgPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    BgAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PurchaseAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FscClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FscSertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseFscClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseFscCertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerOrderId = table.Column<int>(type: "int", nullable: true),
                    SupplierOrderId = table.Column<int>(type: "int", nullable: true),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    PurchaseTransportCost = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    DeliveryTrasnportCost = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    BankExpenses = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Duty = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CustomsExpenses = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Factoring = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    FiscalAgentExpenses = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ProcentComission = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    OtherExpenses = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_CustomerOrders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Descriptions_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "Descriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportCompany = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Vat = table.Column<int>(type: "int", nullable: false),
                    Paid = table.Column<bool>(type: "bit", nullable: false),
                    LoadingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TruckNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountAfterVat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomerOrderId = table.Column<int>(type: "int", nullable: false),
                    SupplierOrderId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_CustomerOrders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrderProductDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CustomerOrderId = table.Column<int>(type: "int", nullable: false),
                    SupplierOrderId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    AutstandingQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    Pallets = table.Column<int>(type: "int", nullable: false),
                    SheetsPerPallet = table.Column<int>(type: "int", nullable: false),
                    TotalSheets = table.Column<int>(type: "int", nullable: false),
                    FscClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FscCertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrderProductDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOrderProductDetails_CustomerOrders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerOrderProductDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerOrderProductDetails_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseProductDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SupplierOrderId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    QuantityM3 = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: true),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    Pallets = table.Column<int>(type: "int", nullable: false),
                    SheetsPerPallet = table.Column<int>(type: "int", nullable: false),
                    TotalSheets = table.Column<int>(type: "int", nullable: false),
                    FscClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FscCertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PurchaseInvoiceId = table.Column<int>(type: "int", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseProductDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseProductDetails_Documents_PurchaseInvoiceId",
                        column: x => x.PurchaseInvoiceId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseProductDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseProductDetails_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrderProductDetailsDocument",
                columns: table => new
                {
                    CustomerInvoicesId = table.Column<int>(type: "int", nullable: false),
                    CustomerOrderProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrderProductDetailsDocument", x => new { x.CustomerInvoicesId, x.CustomerOrderProductsId });
                    table.ForeignKey(
                        name: "FK_CustomerOrderProductDetailsDocument_CustomerOrderProductDetails_CustomerOrderProductsId",
                        column: x => x.CustomerOrderProductsId,
                        principalTable: "CustomerOrderProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrderProductDetailsDocument_Documents_CustomerInvoicesId",
                        column: x => x.CustomerInvoicesId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceProductDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    CustomerOrderId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    Pallets = table.Column<int>(type: "int", nullable: false),
                    SheetsPerPallet = table.Column<int>(type: "int", nullable: false),
                    TotalSheets = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    BgAmount = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    BgPrice = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    FscClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FscCertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellPrice = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    DeliveryCost = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoicedQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    QuantityM3ForCalc = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    CreditNoteId = table.Column<int>(type: "int", nullable: true),
                    DebitNoteId = table.Column<int>(type: "int", nullable: true),
                    CreditNoteQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    CreditNotePallets = table.Column<int>(type: "int", nullable: false),
                    CreditNoteSheetsPerPallet = table.Column<int>(type: "int", nullable: false),
                    CreditNotePrice = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    CreditNoteProductAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CreditNoteBgPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CreditNoteBgAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DebitNoteQuantity = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    DebitNotePallets = table.Column<int>(type: "int", nullable: false),
                    DebitNoteSheetsPerPallet = table.Column<int>(type: "int", nullable: false),
                    DebitNoteAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DebitNotePrice = table.Column<decimal>(type: "decimal(18,5)", precision: 18, scale: 5, nullable: false),
                    DebitNoteBgPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DebitNoteBgAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PurchaseProductDetailsId = table.Column<int>(type: "int", nullable: true),
                    CustomerOrderProductDetailsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceProductDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceProductDetails_CustomerOrderProductDetails_CustomerOrderProductDetailsId",
                        column: x => x.CustomerOrderProductDetailsId,
                        principalTable: "CustomerOrderProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceProductDetails_CustomerOrders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceProductDetails_Documents_CreditNoteId",
                        column: x => x.CreditNoteId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceProductDetails_Documents_DebitNoteId",
                        column: x => x.DebitNoteId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceProductDetails_Documents_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceProductDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceProductDetails_PurchaseProductDetails_PurchaseProductDetailsId",
                        column: x => x.PurchaseProductDetailsId,
                        principalTable: "PurchaseProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_CompanyId",
                table: "BankDetails",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_CurrencyId",
                table: "BankDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_Iban",
                table: "BankDetails",
                column: "Iban",
                unique: true,
                filter: "[Iban] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_SupplierId",
                table: "BankDetails",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderDocument_DocumentsId",
                table: "CustomerOrderDocument",
                column: "DocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderProductDetails_CustomerOrderId",
                table: "CustomerOrderProductDetails",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderProductDetails_ProductId",
                table: "CustomerOrderProductDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderProductDetails_SupplierOrderId",
                table: "CustomerOrderProductDetails",
                column: "SupplierOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderProductDetailsDocument_CustomerOrderProductsId",
                table: "CustomerOrderProductDetailsDocument",
                column: "CustomerOrderProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_CurrencyId",
                table: "CustomerOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_CustomerId",
                table: "CustomerOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_MyCompanyId",
                table: "CustomerOrders",
                column: "MyCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_StatusId",
                table: "CustomerOrders",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderSupplierOrder_SupplierOrdersId",
                table: "CustomerOrderSupplierOrder",
                column: "SupplierOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AddressId",
                table: "Customers",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CurrencyId",
                table: "Documents",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CustomerId",
                table: "Documents",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_MyCompanyId",
                table: "Documents",
                column: "MyCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PurchaseProductDetailsId",
                table: "Documents",
                column: "PurchaseProductDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SupplierId",
                table: "Documents",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SupplierOrderId",
                table: "Documents",
                column: "SupplierOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_CreditNoteId",
                table: "InvoiceProductDetails",
                column: "CreditNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_CustomerOrderId",
                table: "InvoiceProductDetails",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_CustomerOrderProductDetailsId",
                table: "InvoiceProductDetails",
                column: "CustomerOrderProductDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_DebitNoteId",
                table: "InvoiceProductDetails",
                column: "DebitNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_InvoiceId",
                table: "InvoiceProductDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_ProductId",
                table: "InvoiceProductDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProductDetails_PurchaseProductDetailsId",
                table: "InvoiceProductDetails",
                column: "PurchaseProductDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_MyCompanies_AddressId",
                table: "MyCompanies",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerOrderId",
                table: "Payments",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_DocumentId",
                table: "Payments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SupplierOrderId",
                table: "Payments",
                column: "SupplierOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerOrderId",
                table: "Products",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DescriptionId",
                table: "Products",
                column: "DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DocumentId",
                table: "Products",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_GradeId",
                table: "Products",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SizeId",
                table: "Products",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierOrderId",
                table: "Products",
                column: "SupplierOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseProductDetails_ProductId",
                table: "PurchaseProductDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseProductDetails_PurchaseInvoiceId",
                table: "PurchaseProductDetails",
                column: "PurchaseInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseProductDetails_SupplierOrderId",
                table: "PurchaseProductDetails",
                column: "SupplierOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_CustomerOrderId",
                table: "ServiceOrders",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_DocumentId",
                table: "ServiceOrders",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_SupplierOrderId",
                table: "ServiceOrders",
                column: "SupplierOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_CurrencyId",
                table: "SupplierOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_MyCompanyId",
                table: "SupplierOrders",
                column: "MyCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_StatusId",
                table: "SupplierOrders",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_SupplierId",
                table: "SupplierOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_AddressId",
                table: "Suppliers",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_BankDetailId",
                table: "Suppliers",
                column: "BankDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_BankDetails_BankDetailId",
                table: "Suppliers",
                column: "BankDetailId",
                principalTable: "BankDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_PurchaseProductDetails_PurchaseProductDetailsId",
                table: "Documents",
                column: "PurchaseProductDetailsId",
                principalTable: "PurchaseProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankDetails_Currencies_CurrencyId",
                table: "BankDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_Currencies_CurrencyId",
                table: "CustomerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Currencies_CurrencyId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_Currencies_CurrencyId",
                table: "SupplierOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_BankDetails_MyCompanies_CompanyId",
                table: "BankDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_MyCompanies_MyCompanyId",
                table: "CustomerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_MyCompanies_MyCompanyId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_MyCompanies_MyCompanyId",
                table: "SupplierOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_BankDetails_Suppliers_SupplierId",
                table: "BankDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Suppliers_SupplierId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_Suppliers_SupplierId",
                table: "SupplierOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CustomerOrders_CustomerOrderId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Documents_DocumentId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseProductDetails_Documents_PurchaseInvoiceId",
                table: "PurchaseProductDetails");

            migrationBuilder.DropTable(
                name: "CustomerOrderDocument");

            migrationBuilder.DropTable(
                name: "CustomerOrderProductDetailsDocument");

            migrationBuilder.DropTable(
                name: "CustomerOrderSupplierOrder");

            migrationBuilder.DropTable(
                name: "InvoiceProductDetails");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "CustomerOrderProductDetails");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "MyCompanies");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "BankDetails");

            migrationBuilder.DropTable(
                name: "CustomerOrders");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "PurchaseProductDetails");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Descriptions");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "SupplierOrders");

            migrationBuilder.DropTable(
                name: "Statuses");
        }
    }
}
