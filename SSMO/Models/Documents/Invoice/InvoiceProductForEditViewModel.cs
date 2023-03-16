using SSMO.Models.Reports.ProductsStock;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Invoice
{
    public class EditProductForCreditAndDebitViewModel
    {
        public int Id { get; set; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<DescriptionForProductSearchModel> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<SizeForProductSearchModel> Sizes { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public IEnumerable<GradeForProductSearchModel> Grades { get; set; }
        public string Unit { get; set; }
        public string FscClaim { get; set; }
        public string FscSertificate { get; set; }
        public decimal Quantity { get; set; }
        public decimal CreditNotePrice { get; set; }
        public decimal CreditNoteAmount { get; set; }
        public decimal DebitNotePrice { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public decimal Amount { get; set; }
        public ICollection<string> FscCertificates { get; set; }
    }
}
