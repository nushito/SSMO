using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class PurchasePerOrderPaymentsViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAdvance { get; set; }
        public DateTime? DatePaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string PurchaseNumber { get; set; }
        public List<PurchaseAllPaymentsViewModel> PurchasePaymentsDetails { get; set; }
        public decimal NewPaidAmount { get; set; }
        public DateTime? NewDatePaidAmount { get; set; }

    }
}
