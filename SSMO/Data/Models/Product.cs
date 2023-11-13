
using SSMO.Controllers;
using SSMO.Data.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMO.Data.Models
{
   public class Product
    {
        public Product()
        {
            InvoiceProductDetails = new List<InvoiceProductDetails>();
            PurchaseProductDetails = new List<PurchaseProductDetails>();
            CustomerOrderProductDetails = new List<CustomerOrderProductDetails>();
        }
        public int Id { get; init; }
        [Required]
        public int DescriptionId { get; set; }
        [Required]
        public Description Description { get; set; }
        public int SizeId { get; set; }
        [Required]
        public Size Size { get; set; }
        public int GradeId { get; set; }
        [Required]
        public Grade Grade { get; set; }
        [Required]
        [Range(0.0, 9999999999999.99999)]
        public decimal OrderedQuantity { get; set; }
        public decimal? QuantityM3 { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal LoadedQuantity { get; set; }
        public decimal QuantityLeftForPurchaseLoading { get; set; }
        public decimal QuantityAvailableForCustomerOrder { get; set; }
        public decimal SoldQuantity { get; set; }
        public Unit Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal BgPrice { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal Amount { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal BgAmount { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal PurchaseAmount { get; set; }
        public string FscClaim { get; set; }
        public string FscSertificate { get; set; }
        public string PurchaseFscClaim { get; set; }
        public string PurchaseFscCertificate { get; set; }
        public int? CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public int? SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public int? DocumentId { get; set; }
        public Document Document { get; set; }
        public string HsCode { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? PurchaseTransportCost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? DeliveryTrasnportCost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? BankExpenses { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Duty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? CustomsExpenses { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Factoring { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? FiscalAgentExpenses { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ProcentComission { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? OtherExpenses { get; set; }
        public ICollection<CustomerOrderProductDetails> CustomerOrderProductDetails { get; set; }
        public ICollection<PurchaseProductDetails> PurchaseProductDetails { get; set; }
        public ICollection<InvoiceProductDetails> InvoiceProductDetails { get; set; }
                
    }
}
