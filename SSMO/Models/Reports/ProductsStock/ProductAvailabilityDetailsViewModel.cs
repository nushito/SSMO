using System;

namespace SSMO.Models.Reports.ProductsStock
{
    public class ProductAvailabilityDetailsViewModel
    {
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int CustomerOrderId { get; set; }
        public int CustomerOrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal LoadedQuantity { get; set; }
        public string Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal OrderedQuantity { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
    }
}
