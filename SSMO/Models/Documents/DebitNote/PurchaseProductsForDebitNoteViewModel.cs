using SSMO.Data.Enums;
using SSMO.Models.Reports.ProductsStock;
using System.Collections.Generic;

namespace SSMO.Models.Documents.DebitNote
{
    public class PurchaseProductsForDebitNoteViewModel
    {
        public int Id { get; set; }
        public int PurchaseInvoicelId { get; set; }
        public int ProductId { get; set; }
        public bool Checked { get; set; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<DescriptionForProductSearchModel> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<SizeForProductSearchModel> Sizes { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public IEnumerable<GradeForProductSearchModel> Grades { get; set; }
        public string ProductFullDescription { get; set; }
        public Unit Unit { get; set; }
        public string FscClaim { get; set; }
        public string FscSertificate { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal DebitNoteQuantity { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public ICollection<string> FscCertificates { get; set; }
        public int CustomerOrderDetailsId { get; set; }
        public CustomerOrderNumbersByCustomerViewModel CustomerOrderDetail { get; set; }
    }
}
