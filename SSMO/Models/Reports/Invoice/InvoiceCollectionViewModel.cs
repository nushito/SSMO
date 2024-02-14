using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceCollectionViewModel
    {
        public int Id { get; set; }
        public int DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string Incoterms { get; set; }
        public int SupplierOrderId { get; set; }
        public string SupplierName { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
       // public int CustomerOrderId { get; set; }
       // public int OrderConfirmationNumber { get; set; }
        public ICollection<CustomerOrderForInvoiceViewModel> CustomerOrders { get; set; }
        public int CreditToInvoiceId { get; set; }
        public int CreditToInvoiceDocumentNumber { get; set; }
        public DateTime CreditToInvoiceDate { get; set; }
        public decimal CreditNoteTotalAmount { get; set; }
        public int DebitToInvoiceId { get; set; }
        public int DebitToInvoiceDocumentNumber { get; set; }
        public DateTime DebitToInvoiceDate { get; set; }
        public decimal DebitNoteTotalAmount { get; set; }
        public int HeaderId { get; set; }
        public int FooterId { get; set; }
        public string CustomsExportDeclaration { get; set; }
    }
}
