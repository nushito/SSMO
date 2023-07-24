using SSMO.Data.Enums;
using SSMO.Models.Reports.ProductsStock;
using System.Collections.Generic;

namespace SSMO.Models.Reports.DebitNote
{
    public class NewProductsForEditedDebitNoteFormModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<DescriptionForProductSearchModel> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<SizeForProductSearchModel> Sizes { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public IEnumerable<GradeForProductSearchModel> Grades { get; set; }
        public Unit Unit { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public int TotalSheets { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal Profit { get; set; }
        public string VehicleNumber { get; set; }
        public decimal DebitNoteQuantity { get; set; }
        public decimal DebitNotePrice { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public decimal DebitNoteBgPrice { get; set; }
        public decimal DebitNoteBgAmount { get; set; }
        public int DebitNotePallets { get; set; }
        public int DebitNoteSheetsPerPallet { get; set; }
        public ICollection<string> FscCertificates { get; set; }
        public int CustomerOrderId { get; set; }
        public int PurchaseProductDetailsId { get; set; }
        public int? DebitNoteId { get; set; }
        public int? CustomerProductDetailId { get; set; }
        public bool ServiceOrProductQuantity { get; set; }
    }
}
