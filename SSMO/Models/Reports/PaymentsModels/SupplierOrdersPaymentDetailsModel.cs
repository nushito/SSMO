using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierOrdersPaymentDetailsModel
    {
        public int Id { get; set; }
        public string SupplierOrderNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public string DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }
        public string PurchaseCurrency { get; set; }
        public List<SupplierPaymentDetailsViewModel> Payments { get; set; }
        public List<PurchasePerOrderPaymentsViewModel> PurchasePaymentsCollection { get; set; }
    }
}
