﻿using System;

namespace SSMO.Models.Reports.ProductsStock
{
    public class ProductDetailsForEachCustomerOrderViewModel
    {
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int? CustomerOrderId { get; set; }
        public int CustomerOrderNumber { get; set; }
        public string CustomerPoNumber { get; set; }       
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }        
        public string DeliveryAddress { get; set; }       
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal AutstandingQuantity { get; set; }
        public string MyCompanyName { get; set; }
    }
}
