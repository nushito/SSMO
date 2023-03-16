using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierInvoicePaymentDetailsModel
    {
        
        public string PurchaseNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public DateTime DatePaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool PaidStatus { get; set; }
        public string SupplierName { get; set; }
    }
}
