using System;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceCollectionViewModel
    {
        public int Id { get; set; }
        public int DocumentNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string Incoterms { get; set; }
        public int SupplierOrderId { get; set; }
        public string SupplierName { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int CustomerOrderId { get; set; }
        public int OrderConfirmationNumber { get; set; }
    }
}
