using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Services.Documents.Invoice
{
    public class BGProductsForBGInvoiceViewModel
    {
        public int ProductId { get; set; }
        public int DescriptionId { get; set; }
        public string BgDescription { get; set; }
        public IEnumerable<string> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public int GradeId { get; set; }
        [Required]
        public string Grade { get; set; }
        public IEnumerable<string> Grades { get; set; }
        public string Unit { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public int CustomerOrderId { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal InvoicedQuantity { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal BgPrice { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal BgAmount { get; set; }
        public decimal CreditNoteQuantity { get; set; }      
        public decimal CreditNotePrice { get; set; }
        public decimal CreditNoteProductAmount { get; set; }
        public decimal CreditNoteBgPrice { get; set; }
        public decimal CreditNoteBgAmount { get; set; }
        public decimal DebitNoteQuantity { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public decimal DebitNotePrice { get; set; }
        public decimal DebitNoteBgPrice { get; set; }
        public decimal DebitNoteBgAmount { get; set; }        
    }
}
