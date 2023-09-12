using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class EditCustomerOrderPaymentModel
    {
        public int OrderConfirmationNumber { get; set; }
        public decimal Payment { get; set; }
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        public bool PaidStatus { get; set; }
    }
}
