using System;

namespace SSMO.Models.Reports.Products
{
    public class ProductSellDetails
    {
        public string CustomerName { get; set; }
        public int InvoiceNumber { get; set; }
        public string Type { get; set; }
        public int? CreditOrDebitToInvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public string CustomerPoNumber { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Profit { get; set; }
        public string Currency { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string PurchaseInvoiceNumber { get; set; }
    }
}
