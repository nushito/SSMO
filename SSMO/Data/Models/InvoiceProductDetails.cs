using SSMO.Data.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Data.Models
{
    public class InvoiceProductDetails
    {
        public int Id { get; set;}
        public int InvoiceId { get; set; }
        public Document Invoice { get; set; }
        public int? CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Unit Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        public decimal Amount { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal BgAmount { get; set; }
        public decimal BgPrice { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public decimal SellPrice { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal Profit { get; set; }
        public string VehicleNumber { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public decimal QuantityM3ForCalc { get; set; }
        public int? CreditNoteId { get; set; }
        public Document CreditNote { get; set; }
        public int? DebitNoteId { get; set; }
        public Document DebitNote { get; set; }
        public decimal CreditNoteQuantity { get; set; }
        public int CreditNotePallets { get; set; }
        public int CreditNoteSheetsPerPallet { get; set; }
        public decimal CreditNotePrice { get; set; }
        public decimal CreditNoteProductAmount { get; set; }
        public decimal CreditNoteBgPrice { get; set; }
        public decimal CreditNoteBgAmount { get; set; }
        public decimal DebitNoteQuantity { get; set; }
        public int DebitNotePallets { get; set; }
        public int DebitNoteSheetsPerPallet { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public decimal DebitNotePrice { get; set; }
        public decimal DebitNoteBgPrice { get; set; }
        public decimal DebitNoteBgAmount { get; set; }
        public int? PurchaseProductDetailsId { get; set; }
        public PurchaseProductDetails PurchaseProductDetails { get; set; }
        public int? CustomerOrderProductDetailsId { get; set; }
        public CustomerOrderProductDetails CustomerOrderProductDetails { get; set; }
    }
}
