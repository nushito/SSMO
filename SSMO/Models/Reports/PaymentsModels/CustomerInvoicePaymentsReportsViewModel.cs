using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerInvoicePaymentsReportsViewModel
    {
        public const int CustomerOrdersPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalCustomerInvoices { get; set; }
        public string CustomerName { get; set; }
        public IEnumerable<string> CustomerNames { get; set; }
        public IEnumerable<CustomerInvoicePaymentDetailsModel> CustomerPaymentCollection { get; set; }
    }
}
