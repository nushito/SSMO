using SSMO.Data.Enums;
using SSMO.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Purchase
{
    public class PurchaseProductAsSupplierOrderViewModel
    {
        public int Id { get; init; }
        [Required]
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        [Required]
        public int SizeId { get; set; }
        public string Size { get; set; }
        [Required]
        public int GradeId { get; set; }
        public string Grade { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal? OrderedQuantity { get; set; }
        public decimal QuantityLeftForPurchaseLoading { get; set; }
        public Unit Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal PurchasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal BgPrice { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal BgAmount { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal PurchaseAmount { get; set; }
        public string PurchaseFscClaim { get; set; }
        public string PurchaseFscCertificate { get; set; }
        public int? SupplierOrderId { get; set; }
        public string VehicleNumber { get; set; }
        public bool ProductOrNot { get; set; }
    }
}
