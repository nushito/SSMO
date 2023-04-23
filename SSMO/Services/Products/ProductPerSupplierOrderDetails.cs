using System.Collections.Generic;

namespace SSMO.Services.Products
{
    public class ProductPerSupplierOrderDetails
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
        public string Unit { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal OrderedQuantity { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int SupplierOrderId { get; set; }

    }
}
