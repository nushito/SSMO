using System;

namespace SSMO.Models.Reports.ProductsStock
{
    public class PurchaseProductDetailsListViewModel
    {
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; } 
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal LoadedQuantity { get; set; }
        public string Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PurchaseCurrency { get; set; }
        public decimal CostPrice { get; set; }
        public string CostPriceCurrency { get; set; }
    }
}
