using SIA.Data.Enums;
using SSMO.Data.Enums;
using SSMO.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMO.Data.Models
{
   public class Product
    {
      
        public int Id { get; init; }
        [Required]
        public int DescriptionId { get; set; }
        [Required]
        public Description Description { get; set; }
        [Required]
        public Size Size { get; set; }
        [Required]
        public Grade Grade { get; set; }
        [Required]
        [Range(0.0, 9999999999999.99999)]
        public decimal OrderedQuantity { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal LoadedQuantityM3 { get; set; }
        public decimal QuantityM2 { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CostPrice { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal Amount { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }

      
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }

       
        public ICollection<SupplierOrder> SupplierOrders { get; set; }

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
    }
}
