using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.SupplierOrderReportForEdit
{
    public class SupplierOrderDetailsModel
    {
        public int Id { get; set; }
        public string SupplierOrderNumber { get; set; }
        public DateTime Date { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryTerms { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string MyCompanyName { get; set; }
       // public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int CustomerOrderId { get; set; }
        public int CustomerOrderConfirmationNumber { get; set; }
        public string CustomerName { get; set; }
        public bool PaidStatus { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }       
        public IEnumerable<string> PurchaseInvoiceNumbers { get; set; }
        public IEnumerable<ProductsSupplierOrderDetailsViewModel> Products { get; set; }
    }
}
