using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerOrdersPaymentDetailsPerInvoice
    {
        public int Id { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public decimal PaidAvance { get; set; }
        public DateTime DateAdvancePayment { get; set; }
        public ICollection<PaymentViewModel> Payments { get; set; }
        public decimal Balance { get; set; }
    }
}
