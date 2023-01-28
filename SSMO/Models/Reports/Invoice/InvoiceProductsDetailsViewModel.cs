using System.Collections.Generic;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceProductsDetailsViewModel
    {
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public string Unit { get; set; }
        public int DocumentId { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal Amount { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
    }
}
