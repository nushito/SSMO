using System;

namespace SSMO.Models.Reports.Purchase
{
    public class PurchaseInvoicesViewModel
    {
        public int Id { get; set; }
        public string PurchaseNumber { get; set; }
        public DateTime Date { get; set; }
        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int SupplierOrderId { get; set; }
        public string SupplierOrderNumber { get; set; }
        public int CustomerOrderId { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public string CustomerName { get; set; }
        public string Incoterms { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
