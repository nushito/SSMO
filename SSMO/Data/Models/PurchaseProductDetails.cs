using SSMO.Data.Enums;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Data.Models
{
    public class PurchaseProductDetails
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public decimal Quantity { get; set; }
        public decimal? QuantityM3 { get; set; }
        public Unit Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Amount { get; set; }
        public decimal? CalculationCurrencyPrice { get; set; }
        public int PurchaseInvoiceId { get; set; }
        public Document PurchaseInvoice { get; set; }
        public decimal CostPrice { get; set; }
        public int? CostPriceCurrencyId { get; set; }
        public Currency CostPriceCurrency { get; set; }
        public string VehicleNumber { get; set; }
        public ICollection<Document> InvoicesToCustomer { get; set; }
        public ICollection<InvoiceProductDetails> InvoiceProductDetails { get; set; }
    }
}
