using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierOrdersPaymentDetailsModel
    {
        public string SupplierOrderNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public string DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
