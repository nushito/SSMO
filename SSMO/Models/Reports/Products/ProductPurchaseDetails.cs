using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.Products
{
    public class ProductPurchaseDetails
    {
        public string SupplierName { get; set; }
        public string PurchaseInvoice { get; set; }
        public DateTime Date { get; set; }
        public string SupplierOrderNumber { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal AutstandingQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public string Currency { get; set; }
        public string CostPriceCurrency { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public ICollection<ProductSellDetails> SellDetails { get; set; }
    }
}
