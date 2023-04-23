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
        public decimal CreditNoteQuantity { get; set; }
        public decimal CreditNotePrice { get; set; }
        public decimal CreditNoteProductAmount { get; set; }
        public decimal DebitNoteQuantity { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public decimal DebitNotePrice { get; set; }
    }
}
