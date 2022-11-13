using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class EditPurchasePaymentDetails
    {
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public DateTime DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
    }
}
