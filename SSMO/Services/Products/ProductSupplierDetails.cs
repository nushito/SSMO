using System.Collections.Generic;

namespace SSMO.Services.Products
{
    public class ProductSupplierDetails
    {
        public int Id { get; set; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public IEnumerable<string> Grades { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public string PurchaseFscClaim { get; set; }
        public string PurchaseFscCertificate { get; set; }
        public decimal Price { get; set; }

        public decimal PurchasePrice { get; set; }
        public decimal QuantityM3 { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
 
    }
}
