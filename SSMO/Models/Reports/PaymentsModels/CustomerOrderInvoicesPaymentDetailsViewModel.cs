using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerOrderInvoicesPaymentDetailsViewModel
    {
        public int DocumentNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal AdvancePayment { get; set; }
        public DateTime? DateAdvancePayment { get; set; }
        public List<CustomerOrderInvoicesPaymentCollectionViewModel> Payments { get; set; }
    }
}
