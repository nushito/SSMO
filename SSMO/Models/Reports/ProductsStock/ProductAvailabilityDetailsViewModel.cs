using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.ProductsStock
{
    public class ProductAvailabilityDetailsViewModel
    {
        public string Description { get; set; }
        public string Size { get; set; }
        public string Grade { get; set; }
        public List<ProductDetailsForEachCustomerOrderViewModel> CustomerProductsDetails { get; set; }
        public List<PurchaseProductDetailsListViewModel> PurchaseProductDetails { get; set; }
        public decimal LoadedQuantity { get; set; }        
        public decimal QuantityOnStock { get; set; }
        public string Unit { get; set; }
        public string SupplierName { get; set; }
        public string SupplierOrderNumber { get; set; }
    }
}
