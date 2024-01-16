using SSMO.Data.Enums;
using System.Collections.Generic;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceProductsDetailsViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerOrderId { get; set; }
        
        public string Description { get; set; }
       
        public string Size { get; set; }
       
        public string Grade { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public Unit Unit { get; set; }
        public int InvoiceId { get; set; }
        public int? CreditNoteId { get; set; }
        public int? DebitNoteId { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public int TotalSheets { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal SellPrice { get; set; }        
        public decimal Amount { get; set; }
        public decimal BgAmount { get; set; }
        public decimal BgPrice { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal Profit { get; set; }
        public decimal CreditNoteQuantity { get; set; }
        public int CreditNotePallets { get; set; }
        public int CreditNoteSheetsPerPallet { get; set; }
        public decimal CreditNotePrice { get; set; }
        public decimal CreditNoteProductAmount { get; set; }
        public decimal CreditNoteBgPrice { get; set; }
        public decimal CreditNoteBgAmount { get; set; }
        public decimal DebitNoteQuantity { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public decimal DebitNotePrice { get; set; }
        public decimal DebitNoteBgPrice { get; set; }
        public decimal DebitNoteBgAmount { get; set; }
        public int? PurchaseProductDetailsId { get; set; }
        public string HsCode { get; set; }
    }
}
