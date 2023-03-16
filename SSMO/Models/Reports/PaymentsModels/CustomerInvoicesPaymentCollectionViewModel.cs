using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerInvoicesPaymentCollectionViewModel
    {
        public int TotalInvoices { get; set; }
        public IEnumerable<CustomerInvoicePaymentDetailsModel> CustomerInvoices { get; set; }
    }
}
