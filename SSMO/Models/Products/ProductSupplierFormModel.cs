
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SSMO.Models.Products
{
    public class ProductSupplierFormModel 
    {
        public int Id { get; init; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public int GradeId { get; set; }
        [Required]
        public string Grade { get; set; }
        public IEnumerable<string> Grades { get; set; }
        public string PurchaseFscClaim { get; set; }
        public string PurchaseFscCertificate { get; set; }
        public string SupplierFscCertNumber { get; set; }
        public int CustomerOrderId { get; set; }
        public int SupplierOrderId { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal PurchasePrice { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal QuantityM3  { get; set; }


    }
    }
