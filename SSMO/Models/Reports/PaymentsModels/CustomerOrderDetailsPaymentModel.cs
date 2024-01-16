using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerOrderDetailsPaymentModel
    {
        public int Id { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public DateTime Date { get; set; }        
        public decimal Balance { get; set; }
        public decimal TotalAmount { get; set; }       
        public bool PaidStatus { get; set; }
        public string SupplierName { get; set; }
        public IEnumerable<CustomerOrderPaymentsDetailsViewModel> CustomerOrderPayments { get; set; }
        public IEnumerable<CustomerOrderInvoicesPaymentDetailsViewModel> InvoicesDetails { get; set; }

    }
}
