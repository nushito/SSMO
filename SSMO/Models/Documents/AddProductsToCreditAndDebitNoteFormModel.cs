using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.ProductsStock;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents
{
    public class AddProductsToCreditAndDebitNoteFormModel
    {
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
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public ICollection<string> FscCertificates { get; set; }
        public int CustomerOrderId { get; set; }
        public ICollection<CustomerOrderNumbersByCustomerViewModel> CustomerOrderNumbers { get; set; }
    }
}